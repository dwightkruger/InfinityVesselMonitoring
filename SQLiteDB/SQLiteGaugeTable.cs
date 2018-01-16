//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Threading;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Utilities;
using Microsoft.Graphics.Canvas.Text;
using SQLitePCL;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteGaugeTable : SQLiteVesselTableBase<ItemRow, Int64>, IGaugeTable
    {
        public SQLiteGaugeTable(IVesselDB myVesselDB) : base(myVesselDB)
        {
            PrimaryKeyName = "GaugeId";
            TableName = "GaugeTable";
        }

        protected override string GetCreateTableSql()
        {
            return
                "CREATE TABLE IF NOT EXISTS " + TableName + "\n" +
                "(\n" +
                    "GaugeId                 INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT, \n " +
                    "ChangeDate              DATETIME NOT NULL, \n " +
                    "GaugeType               INTEGER  NOT NULL, \n " +
                    "PageId                  INTEGER  NOT NULL, \n " +
                    "SensorId                INTEGER  NOT NULL, \n " +
                    "GaugeLeft               FLOAT    NOT NULL, \n " +
                    "GaugeTop                FLOAT    NOT NULL, \n " +
                    "GaugeHeight             FLOAT    NOT NULL, \n " +
                    "GaugeWidth              FLOAT    NOT NULL, \n " +
                    "GaugeColor              INTEGER  NOT NULL, \n " +
                    "Divisions               INTEGER  NOT NULL, \n " +
                    "MinorTicsPerMajorTic    INTEGER  NOT NULL, \n " +
                    "MediumTicsPerMajorTic   INTEGER  NOT NULL, \n " +
                    "Resolution              INTEGER  NOT NULL, \n " +
                    "GaugeOutlineVisibility  INTEGER  NOT NULL, \n " +
                    "MiddleCircleDelta       INTEGER  NOT NULL, \n " +
                    "InnerCircleDelta        INTEGER  NOT NULL, \n " +
                    "ValueFontSize           FLOAT    NOT NULL, \n " +
                    "UnitsFontSize           FLOAT    NOT NULL, \n " +
                    "Units                   INTEGER  NOT NULL, \n " +
                    "MajorTicLength          FLOAT    NOT NULL, \n " +
                    "MediumTicLength         FLOAT    NOT NULL, \n " +
                    "MinorTicLength          FLOAT    NOT NULL, \n " +
                    "Text                    TEXT     NOT NULL, \n " +
                    "TextFontSize            FLOAT    NOT NULL, \n " +
                    "TextFontColor           INTEGER  NOT NULL, \n " +
                    "TextAngle               FLOAT    NOT NULL, \n " +
                    "TextHorizontalAlignment INTEGER  NOT NULL, \n " +
                    "TextVerticalAlignment   INTEGER  NOT NULL, \n " +
                    "Properties              TEXT     NOT NULL, \n " +
                    "FOREIGN KEY(PageId) REFERENCES GaugePageTable(PageId) \n" +
                ") ";
        }

        protected override string GetSelectAllSql()
        {
            return
                "SELECT " +
                    "GaugeId, " +
                    "ChangeDate, " +
                    "GaugeType, " +
                    "PageId, " +
                    "SensorId, " +
                    "GaugeLeft, " +
                    "GaugeTop, " +
                    "GaugeHeight, " +
                    "GaugeWidth, " +
                    "GaugeColor, " +
                    "Divisions, " +
                    "MinorTicsPerMajorTic, " +
                    "MediumTicsPerMajorTic, " +
                    "Resolution, " +
                    "GaugeOutlineVisibility, " +
                    "MiddleCircleDelta, " +
                    "InnerCircleDelta, " +
                    "ValueFontSize, " +
                    "UnitsFontSize, " +
                    "Units, " +
                    "MajorTicLength, " +
                    "MediumTicLength, " +
                    "MinorTicLength, " +
                    "Text, " +
                    "TextFontSize, " +
                    "TextFontColor, " +
                    "TextAngle, " +
                    "TextHorizontalAlignment, " +
                    "TextVerticalAlignment, " +
                    "Properties " +
                "FROM " + TableName;
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, Int64 key)
        {
            statement.Bind("@" + PrimaryKeyName, key);
        }


        protected override void FillInsertItemStatement(ISQLiteStatement statement, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.Now.ToUniversalTime());

            statement.Bind("@ChangeDate",              SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("ChangeDate")));
            statement.Bind("@GaugeType",               Convert.ToInt32(itemRow.Field<GaugeTypeEnum>("GaugeType")));
            statement.Bind("@PageId",                  itemRow.Field<Int64>("PageId"));
            statement.Bind("@SensorId",                itemRow.Field<Int64>("SensorId"));
            statement.Bind("@GaugeLeft",               itemRow.Field<double>("GaugeLeft"));
            statement.Bind("@GaugeTop",                itemRow.Field<double>("GaugeTop"));
            statement.Bind("@GaugeHeight",             itemRow.Field<double>("GaugeHeight"));
            statement.Bind("@GaugeWidth",              itemRow.Field<double>("GaugeWidth"));
            statement.Bind("@GaugeColor",              Utilities.ColorToI64(itemRow.Field<Color>("GaugeColor")));
            statement.Bind("@Divisions",               itemRow.Field<int>("Divisions"));
            statement.Bind("@MinorTicsPerMajorTic",    itemRow.Field<int>("MinorTicsPerMajorTic"));
            statement.Bind("@MediumTicsPerMajorTic",   itemRow.Field<int>("MediumTicsPerMajorTic"));
            statement.Bind("@Resolution",              itemRow.Field<int>("Resolution"));
            statement.Bind("@GaugeOutlineVisibility",  Convert.ToInt32(itemRow.Field<Windows.UI.Xaml.Visibility>("GaugeOutlineVisibility")));
            statement.Bind("@MiddleCircleDelta",       itemRow.Field<int>("MiddleCircleDelta"));
            statement.Bind("@InnerCircleDelta",        itemRow.Field<int>("InnerCircleDelta"));
            statement.Bind("@ValueFontSize",           itemRow.Field<double>("ValueFontSize"));
            statement.Bind("@UnitsFontSize",           itemRow.Field<double>("UnitsFontSize"));
            statement.Bind("@Units",                   Convert.ToInt32(itemRow.Field<Units>("Units")));
            statement.Bind("@MajorTicLength",          itemRow.Field<double>("MajorTicLength"));
            statement.Bind("@MediumTicLength",         itemRow.Field<double>("MediumTicLength"));
            statement.Bind("@MinorTicLength",          itemRow.Field<double>("MinorTicLength"));
            statement.Bind("@Text",                    itemRow.Field<string>("Text"));
            statement.Bind("@TextFontSize",            itemRow.Field<double>("TextFontSize"));
            statement.Bind("@TextFontColor",           Utilities.ColorToI64(itemRow.Field<Color>("TextFontColor")));
            statement.Bind("@TextAngle",               itemRow.Field<double>("TextAngle"));
            statement.Bind("@TextHorizontalAlignment", Convert.ToInt32(itemRow.Field<CanvasHorizontalAlignment>("TextHorizontalAlignment")));
            statement.Bind("@TextVerticalAlignment",   Convert.ToInt32(itemRow.Field<CanvasVerticalAlignment>("TextVerticalAlignment")));
            statement.Bind("@Properties",              itemRow.Field<string>("Properties"));
        }

        protected override void FillUpdateItemStatement(ISQLiteStatement statement, Int64 key, ItemRow itemRow)
        {
            statement.Bind("@" + PrimaryKeyName, itemRow.Field<Int64>(PrimaryKeyName));

            FillInsertItemStatement(statement, itemRow);
        }

        protected override void LoadTableRow(ISQLiteStatement statement, ItemTable itemTable)
        {
            ItemRow itemRow = this.CreateRow(itemTable);

            itemRow.SetField<Int64>(PrimaryKeyName,             (Int64)Convert.ToInt64(statement[00]));
            itemRow.SetField<DateTime>("ChangeDate",            (DateTime)DateTime.Parse((string)statement[01]));
            itemRow.SetField<GaugeTypeEnum>("GaugeType",        (GaugeTypeEnum)Convert.ToInt32(statement[02]));
            itemRow.SetField<Int64>("PageId",                   (Int64)Convert.ToInt64(statement[03]));
            itemRow.SetField<Int64>("SensorId",                 (Int64)Convert.ToInt64(statement[04]));
            itemRow.SetField<double>("GaugeLeft",               (double)Convert.ToDouble(statement[05]));
            itemRow.SetField<double>("GaugeTop",                (double)Convert.ToDouble(statement[06]));
            itemRow.SetField<double>("GaugeHeight",             (double)Convert.ToDouble(statement[07]));
            itemRow.SetField<double>("GaugeWidth",              (double)Convert.ToDouble(statement[08]));
            itemRow.SetField<Color>("GaugeColor",               (Color)Utilities.ColorFromI64(Convert.ToInt64(statement[09])));
            itemRow.SetField<int>("Divisions",                  (int)Convert.ToInt32(statement[10]));
            itemRow.SetField<int>("MinorTicsPerMajorTic",       (int)Convert.ToInt32(statement[11]));
            itemRow.SetField<int>("MediumTicsPerMajorTic",      (int)Convert.ToInt32(statement[12]));
            itemRow.SetField<int>("Resolution",                 (int)Convert.ToInt32(statement[13]));
            itemRow.SetField<int>("GaugeOutlineVisibility",     (int)Convert.ToInt32(statement[14]));
            itemRow.SetField<int>("MiddleCircleDelta",          (int)Convert.ToInt32(statement[15]));
            itemRow.SetField<int>("InnerCircleDelta",           (int)Convert.ToInt32(statement[16]));
            itemRow.SetField<double>("ValueFontSize",           (double)Convert.ToDouble(statement[17]));
            itemRow.SetField<double>("UnitsFontSize",           (double)Convert.ToDouble(statement[18]));
            itemRow.SetField<Units>("Units",                    (Units)Convert.ToInt32(statement[19]));
            itemRow.SetField<double>("MajorTicLength",          (double)Convert.ToDouble(statement[20]));
            itemRow.SetField<double>("MediumTicLength",         (double)Convert.ToDouble(statement[21]));
            itemRow.SetField<double>("MinorTicLength",          (double)Convert.ToDouble(statement[22]));
            itemRow.SetField<string>("Text",                    (string)Convert.ToString(statement[23]));
            itemRow.SetField<double>("TextFontSize",            (double)Convert.ToDouble(statement[24]));
            itemRow.SetField<Color>("TextFontColor",            (Color)Utilities.ColorFromI64((Int64)statement[25])); 
            itemRow.SetField<double>("TextAngle",               (double)Convert.ToDouble(statement[26]));
            itemRow.SetField<CanvasHorizontalAlignment>("TextHorizontalAlignment", (CanvasHorizontalAlignment)Convert.ToInt32(statement[27]));
            itemRow.SetField<CanvasVerticalAlignment>("TextVerticalAlignment",     (CanvasVerticalAlignment)Convert.ToInt32(statement[28]));
            itemRow.SetField<string>("Properties",              (string)statement[29]);

            itemTable.Rows.Add(itemRow);
            itemRow.AcceptChanges();
        }

        protected override void CreateTableSchema(ItemTable itemTable)
        {
            itemTable.Columns.Add(PrimaryKeyName,            typeof(Int64));
            itemTable.Columns.Add("ChangeDate",              typeof(DateTime));
            itemTable.Columns.Add("GaugeType",               typeof(GaugeTypeEnum));
            itemTable.Columns.Add("PageId",                  typeof(Int64));
            itemTable.Columns.Add("SensorId",                typeof(Int64));
            itemTable.Columns.Add("GaugeLeft",               typeof(double));
            itemTable.Columns.Add("GaugeTop",                typeof(double));
            itemTable.Columns.Add("GaugeHeight",             typeof(double));
            itemTable.Columns.Add("GaugeWidth",              typeof(double));
            itemTable.Columns.Add("GaugeColor",              typeof(Color));
            itemTable.Columns.Add("Divisions",               typeof(int));
            itemTable.Columns.Add("MinorTicsPerMajorTic",    typeof(int));
            itemTable.Columns.Add("MediumTicsPerMajorTic",   typeof(int));
            itemTable.Columns.Add("Resolution",              typeof(int));
            itemTable.Columns.Add("GaugeOutlineVisibility",  typeof(int));
            itemTable.Columns.Add("MiddleCircleDelta",       typeof(int));
            itemTable.Columns.Add("InnerCircleDelta",        typeof(int));
            itemTable.Columns.Add("ValueFontSize",           typeof(double));
            itemTable.Columns.Add("UnitsFontSize",           typeof(double));
            itemTable.Columns.Add("Units",                   typeof(Units));
            itemTable.Columns.Add("MajorTicLength",          typeof(double));
            itemTable.Columns.Add("MediumTicLength",         typeof(double));
            itemTable.Columns.Add("MinorTicLength",          typeof(double));
            itemTable.Columns.Add("Text",                    typeof(string));
            itemTable.Columns.Add("TextFontSize",            typeof(double));
            itemTable.Columns.Add("TextFontColor",           typeof(Color));
            itemTable.Columns.Add("TextAngle",               typeof(double));
            itemTable.Columns.Add("TextHorizontalAlignment", typeof(CanvasHorizontalAlignment));
            itemTable.Columns.Add("TextVerticalAlignment",   typeof(CanvasVerticalAlignment));
            itemTable.Columns.Add("Properties",              typeof(string));

            itemTable.Columns[PrimaryKeyName].DefaultValue            = -1L;
            itemTable.Columns["ChangeDate"].DefaultValue              = DateTime.Now.ToUniversalTime();
            itemTable.Columns["GaugeType"].DefaultValue               = GaugeTypeEnum.Unknown;
            itemTable.Columns["PageId"].DefaultValue                  = -1L;
            itemTable.Columns["SensorId"].DefaultValue                = -1L;
            itemTable.Columns["GaugeLeft"].DefaultValue               = -1D;    // -1 means not positioned
            itemTable.Columns["GaugeTop"].DefaultValue                = -1D;    // -1 means not positioned
            itemTable.Columns["GaugeHeight"].DefaultValue             = 300D;
            itemTable.Columns["GaugeWidth"].DefaultValue              = 200D;
            itemTable.Columns["GaugeColor"].DefaultValue              = Colors.White;
            itemTable.Columns["Divisions"].DefaultValue               = 7;
            itemTable.Columns["MinorTicsPerMajorTic"].DefaultValue    = 3;
            itemTable.Columns["MediumTicsPerMajorTic"].DefaultValue   = 6;
            itemTable.Columns["Resolution"].DefaultValue              = 0;
            itemTable.Columns["GaugeOutlineVisibility"].DefaultValue  = Convert.ToInt32(Windows.UI.Xaml.Visibility.Visible);
            itemTable.Columns["MiddleCircleDelta"].DefaultValue       = 70;
            itemTable.Columns["InnerCircleDelta"].DefaultValue        = 30;
            itemTable.Columns["ValueFontSize"].DefaultValue           = 60D;
            itemTable.Columns["UnitsFontSize"].DefaultValue           = 60D;
            itemTable.Columns["Units"].DefaultValue                   = Units.AmpHrs;
            itemTable.Columns["MajorTicLength"].DefaultValue          = 18D;
            itemTable.Columns["MediumTicLength"].DefaultValue         = 12D;
            itemTable.Columns["MinorTicLength"].DefaultValue          = 6D;
            itemTable.Columns["Text"].DefaultValue                    = string.Empty;
            itemTable.Columns["TextFontSize"].DefaultValue            = 12D;
            itemTable.Columns["TextFontColor"].DefaultValue           = Colors.Red;
            itemTable.Columns["TextAngle"].DefaultValue               = 0D;
            itemTable.Columns["TextHorizontalAlignment"].DefaultValue = CanvasHorizontalAlignment.Left;
            itemTable.Columns["TextVerticalAlignment"].DefaultValue   = CanvasVerticalAlignment.Top;
            itemTable.Columns["Properties"].DefaultValue              = string.Empty;
        }

        override protected string GetInsertItemSql()
        {
            return
                "INSERT INTO " + TableName +
                          " ( " +
                                "ChangeDate, " +
                                "GaugeType, " +
                                "PageId, " +
                                "SensorId, " +
                                "GaugeLeft, " +
                                "GaugeTop, " +
                                "GaugeHeight, " +
                                "GaugeWidth, " +
                                "GaugeColor, " +
                                "Divisions," +
                                "MinorTicsPerMajorTic," +
                                "MediumTicsPerMajorTic," +
                                "Resolution," +
                                "GaugeOutlineVisibility," +
                                "MiddleCircleDelta," +
                                "InnerCircleDelta," +
                                "ValueFontSize," +
                                "UnitsFontSize," +
                                "Units, " +
                                "MajorTicLength," +
                                "MediumTicLength," +
                                "MinorTicLength," +
                                "Text, " +
                                "TextFontSize, " +
                                "TextFontColor, " +
                                "TextAngle, " +
                                "TextHorizontalAlignment, " +
                                "TextVerticalAlignment, " +
                                "Properties " +
                          " ) " +
                          " VALUES " +
                          " ( " +
                                "@ChangeDate, " +
                                "@GaugeType, " +
                                "@PageId, " +
                                "@SensorId, " +
                                "@GaugeLeft, " +
                                "@GaugeTop, " +
                                "@GaugeHeight, " +
                                "@GaugeColor, " +
                                "@GaugeWidth, " +
                                "@Divisions, " +
                                "@MinorTicsPerMajorTic, " +
                                "@MediumTicsPerMajorTic, " +
                                "@Resolution, " +
                                "@GaugeOutlineVisibility, " +
                                "@MiddleCircleDelta, " +
                                "@InnerCircleDelta, " +
                                "@ValueFontSize, " +
                                "@UnitsFontSize, " +
                                "@Units, " +
                                "@MajorTicLength, " +
                                "@MediumTicLength, " +
                                "@MinorTicLength, " +
                                "@Text, " +
                                "@TextFontSize, " +
                                "@TextFontColor, " +
                                "@TextAngle, " +
                                "@TextHorizontalAlignment, " +
                                "@TextVerticalAlignment, " +
                                "@Properties " +
                          " ) ";
        }

        override protected string GetUpdateItemSql()
        {
            return "UPDATE " + TableName + "\n" +
                   " SET \n" +
                   "    ChangeDate              = @ChangeDate             , \n" +
                   "    GaugeType               = @GaugeType              , \n" +
                   "    PageId                  = @PageId                 , \n" +
                   "    SensorId                = @SensorId               , \n" +
                   "    GaugeLeft               = @GaugeLeft              , \n" +
                   "    GaugeTop                = @GaugeTop               , \n" +
                   "    GaugeHeight             = @GaugeHeight            , \n" +
                   "    GaugeWidth              = @GaugeWidth             , \n" +
                   "    GaugeColor              = @GaugeColor             , \n" +
                   "    Divisions               = @Divisions              , \n" +
                   "    MinorTicsPerMajorTic    = @MinorTicsPerMajorTic   , \n" +
                   "    MediumTicsPerMajorTic   = @MediumTicsPerMajorTic  , \n" +
                   "    Resolution              = @Resolution             , \n" +
                   "    GaugeOutlineVisibility  = @GaugeOutlineVisibility , \n" +
                   "    MiddleCircleDelta       = @MiddleCircleDelta      , \n" +
                   "    InnerCircleDelta        = @InnerCircleDelta       , \n" +
                   "    ValueFontSize           = @ValueFontSize          , \n" +
                   "    UnitsFontSize           = @UnitsFontSize          , \n" +
                   "    Units                   = @Units                  , \n" +
                   "    MajorTicLength          = @MajorTicLength         , \n" +
                   "    MediumTicLength         = @MediumTicLength        , \n" +
                   "    MinorTicLength          = @MinorTicLength         , \n" +
                   "    Text                    = @Text                   , \n" +
                   "    TextFontSize            = @TextFontSize           , \n" +
                   "    TextFontColor           = @TextFontColor          , \n" +
                   "    TextAngle               = @TextAngle              , \n" +
                   "    TextHorizontalAlignment = @TextHorizontalAlignment, \n" +
                   "    TextVerticalAlignment   = @TextVerticalAlignment  , \n" +
                   "    Properties              = @Properties   \n" +
                   " WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }


        override protected string GetDeleteItemSql()
        {
            return "DELETE FROM " + TableName +
                   "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }

        virtual async public Task BeginFindByGaugePageId(Int64 gaugePageId, Action<ItemTable> sucessCallback, Action<Exception> failureCallback)
        {
            ItemTable results = null;

            await Task.Run(() =>
            {
                try
                {
                    results = new ItemTable();
                    this.CreateTableSchema(results);

                    string query = "SELECT * FROM " + TableName +
                                   " WHERE PageId = " + gaugePageId.ToString();

                    using (var statement = sqlConnection.Prepare(query))
                    {
                        while (statement.Step() == SQLiteResult.ROW)
                        {
                            this.LoadTableRow(statement, results);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    failureCallback(ex);
                }

                sucessCallback(results);
            });
        }
    }
}
