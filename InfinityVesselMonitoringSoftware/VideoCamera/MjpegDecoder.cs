//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using Lumia.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace InfinityVesselMonitoringSoftware.VideoCamera
{
    public class MjpegDecoder
    {
        public Bitmap Bitmap { get; set; }

        // magic 2 byte header for JPEG images
        private byte[] JpegHeader = new byte[] { 0xff, 0xd8 };

        // pull down 1024 bytes at a time
        private const int ChunkSize = 1024;

        // used to cancel reading the stream
        private bool _streamActive;

        // current encoded JPEG image
        public byte[] CurrentFrame { get; private set; }

        // WPF and Silverlight
        public BitmapImage BitmapImage { get; set; }

        // used to marshal back to UI thread
        private SynchronizationContext _context;

        // event to get the buffer above handed to you
        internal EventHandler<FrameReadyEventArgs> FrameReady;

        private HttpClient _request;

        private bool IsProcessingCommand { get; set; }

        public MjpegDecoder()
        {
            _context = SynchronizationContext.Current;

            BitmapImage = new BitmapImage();
            IsProcessingCommand = false;
        }

        public void ParseStream(Uri uri)
        {
            ParseStream(uri, null, null);
        }

        async public void ParseStream(Uri uri, string username, string password)
        {
            if (_request != null)
            {
                this.StopStream();
            }

            //_request = (HttpWebRequest)WebRequest.Create(uri);
            _request = new HttpClient();
            HttpResponseMessage response = await _request.PostAsync(uri.ToString(), null);
            response.EnsureSuccessStatusCode();

            byte[] imageBuffer = await _request.GetByteArrayAsync(uri);

            OnGetResponse(imageBuffer, response);

            //if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            //    _request.Credentials = new NetworkCredential(username, password);

            //// asynchronously get a response
            ////_request.KeepAlive = false;
            //_request.Timeout = System.Threading.Timeout.Infinite;
            //_request.ProtocolVersion = HttpVersion.Version10;
            //_request.BeginGetResponse(OnGetResponse, _request);
        }

        public void StopStream()
        {
            if (_request != null)
            {
                _request.Dispose();
                _request = null;
            }

            _streamActive = false;
        }

        private void OnGetResponse(byte[] imageBuffer, HttpResponseMessage resp)
        {
            Stream s;
            byte[] buff;

            try
            {
                // find our magic boundary value
                IEnumerable<string> headerValues = resp.Headers.GetValues("Content-Type");
                string contentType = headerValues.FirstOrDefault<string>();

                if (!resp.Headers.Contains("Content-Type") && !contentType.Contains("="))
                    throw new Exception("Invalid content-type header. The camera is likely not returning a proper MJPEG stream.");

                string boundary = contentType.Split('=')[1].Replace("\"", "");
                byte[] boundaryBytes = Encoding.UTF8.GetBytes(boundary.StartsWith("--") ? boundary : "--" + boundary);

                s = new MemoryStream(imageBuffer);
                BinaryReader br = new BinaryReader(s);

                _streamActive = true;

                buff = br.ReadBytes(ChunkSize);

                while (_streamActive)
                {
                    int size;

                    // find the JPEG header
                    int imageStart = buff.Find(JpegHeader);

                    if (imageStart != -1)
                    {
                        // copy the start of the JPEG image to the imageBuffer
                        size = buff.Length - imageStart;
                        Array.Copy(buff, imageStart, imageBuffer, 0, size);

                        while (true)
                        {
                            buff = br.ReadBytes(ChunkSize);

                            // find the boundary text
                            int imageEnd = buff.Find(boundaryBytes);
                            if (imageEnd != -1)
                            {
                                // copy the remainder of the JPEG to the imageBuffer
                                Array.Copy(buff, 0, imageBuffer, size, imageEnd);
                                size += imageEnd;

                                // create a single JPEG frame
                                CurrentFrame = new byte[size];
                                Array.Copy(imageBuffer, 0, CurrentFrame, 0, size);
                                ProcessFrame(CurrentFrame);

                                // copy the leftover data to the start
                                Array.Copy(buff, imageEnd, buff, 0, buff.Length - imageEnd);

                                // fill the remainder of the buffer with new data and start over
                                byte[] temp = br.ReadBytes(imageEnd);

                                Array.Copy(temp, 0, buff, buff.Length - imageEnd, temp.Length);
                                break;
                            }

                            // copy all of the data to the imageBuffer
                            Array.Copy(buff, 0, imageBuffer, size, buff.Length);
                            size += buff.Length;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
            finally
            {
                if (resp != null) resp.Dispose();
            }
        }

        private void ProcessFrame(byte[] frameBuffer)
        {

            _context.Post(delegate
            {
                // create a simple GDI+ happy Bitmap
                InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
                DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0));
                writer.WriteBytes(frameBuffer);
                writer.StoreAsync().GetResults();
                this.BitmapImage = new BitmapImage();
                this.BitmapImage.SetSource(ms);

                // tell whoever's listening that we have a frame to draw
                if (FrameReady != null)
                    FrameReady(this, new FrameReadyEventArgs { FrameBuffer = CurrentFrame, BitmapImage = BitmapImage });
            }, null);
        }
    }

    public static class Extensions
    {
        public static int Find(this byte[] buff, byte[] search)
        {
            // enumerate the buffer but don't overstep the bounds
            for (int start = 0; start < buff.Length - search.Length; start++)
            {
                // we found the first character
                if (buff[start] == search[0])
                {
                    int next;

                    // traverse the rest of the bytes
                    for (next = 1; next < search.Length; next++)
                    {
                        // if we don't match, bail
                        if (buff[start + next] != search[next])
                            break;
                    }

                    if (next == search.Length)
                        return start;
                }
            }

            // not found
            return -1;
        }
    }
}