//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////   

using InfinityGroup.VesselMonitoring.Globals;
using System;
using System.Collections;
using System.Collections.Generic;

namespace InfinityGroup.VesselMonitoring.Utilities
{
    public enum UnitType
    {
        Uninitialized = 0,
        Angle,
        AngularRate,
        Current,
        Date,
        Density,
        ElectricCharge,
        Frequency,
        Length,
        Mass,
        Mileage,
        Null,
        Percent,
        Power,
        Pressure,
        Rotation,
        Speed,
        Switch,
        State,
        Temperature,
        Time,
        Volume,
        Voltage,
        VolumeFlow,
    }

    public enum Units
    {
        Other = 0,
        LitersPerNauticalMile,
        LitersPerKilometer,
        GallonsUSPerHr,
        GallonsUSPerMinute,
        GallonsUKPerNauticalMile,
        Fahrenheit,
        Celsius,
        Kelvin,
        GallonsUS,
        GallonsUK,
        Liters,
        Pounds,
        Kg,
        Km,
        Miles,
        Feet,
        Meters,
        Fathoms,
        NauticalMiles,
        Yards,
        PSI,
        KPa,
        MmHg,
        Volts,
        Amps,
        AmpHrs,
        Hertz,
        Degrees,
        Hours,
        Percent,
        Watts,
        Knots,
        MilesPerHour,
        RPM,
        ONOFF,
        ON,
        OFF,
        InchesHg,
        MetersPerSecond,
        Pascals,
        CubicMetersPerHr,
        GallonsUSPerNauticalMile,
        GallonsUKPerHr,
        GallonsUKPerMinute,
        LitersPerHr,
        LitersPerMinute,
        Minutes,
        Seconds,
        Radians,
        KilometersPerHour,
        Date,
        RadiansPerSecond,
        DegreesPerSecond,
        DegreesPerMinute,
        KiloWatts,
        Deciliters,
        Null,
        KgPerCubicMeter,
        MetricTons,
        KgPerHour,
        PoundsPerHour,
        KgPerLiter,
        CubicMeters,
        MM,
        Bar
    }

    public class UnitItem
    {
        internal double Scale;
        internal double Shift;
        public UnitType Type;
        public Units Units;

        private const string _ampHrs = "AHrs";
        private const string _amps = "A";
        private const string _bar = "Bar";
        private const string _cubicMetersPerHour = "Cu-m/hr";
        private const string _cubicMeters = "Cu-m";
        private const string _date = "Date";
        private const string _deciliters = "dl";
        private const string _degrees = "°";
        private const string _degreesPerSecond = "°/Sec";
        private const string _degreesPerMinute = "°/Min";
        private const string _degreeC = "°C";
        private const string _degreeF = "°F";
        private const string _degreeK = "°K";
        private const string _fathoms = "Fathoms";
        private const string _feet = "Ft";
        private const string _gallonsUS = "Gal";
        private const string _gallonsUK = "Gal(UK)";
        private const string _gallonsUSPerNauticalMile = "Gal/NM";
        private const string _gallonsUSPerHour = "GPH";
        private const string _gallonsUSPerMinute = "Gal/Min";
        private const string _gallonsUKPerNauticalMile = "Gal(UK)/NM";
        private const string _gallonsUKPerHour = "Gal(UK)/Hr";
        private const string _gallonsUKPerMinute = "Gal(UK)/Min";
        private const string _hertz = "Hz";
        private const string _hours = "Hrs";
        private const string _inchesOfMercury = "\"Hg";
        private const string _kilopascal = "KPa";
        private const string _kilograms = "Kg";
        private const string _kilogramsPerCubicMeter = "Kg/M^3";
        private const string _kilogramsPerLiter = "Kg/Ltr";
        private const string _kilogramsPerHour = "Kg/Hr";
        private const string _kilometers = "Km";
        private const string _kilometersPerHour = "KPH";
        private const string _liters = "Ltr";
        private const string _litersPerHour = "LPH";
        private const string _litersPerMinute = "LPM";
        private const string _litersPerNauticalMile = "Ltr/NM";
        private const string _milesPerHour = "MPH";
        private const string _millimetreOfMercury = "mmHg";
        private const string _millimetres = "mm";
        private const string _minutes = "Min";
        private const string _meters = "M";
        private const string _metersPerSecond = "M/S";
        private const string _metricTons = "mTons";
        private const string _miles = "Miles";
        private const string _nauticalMiles = "Nm";
        private const string _nauticalMilesPerHour = "Kts";
        private const string _null = "";
        private const string _OFF = "OFF";
        private const string _ON = "ON";
        private const string _ONOFF = "ON/OFF";
        private const string _pascals = "Pa";
        private const string _percent = "%";
        private const string _pounds = "Lbs";
        private const string _poundsPerHour = "Lbs/Hr";
        private const string _PSI = "PSI";
        private const string _radians = "Radians";
        private const string _radiansPerSecond = "Rad/S";
        private const string _RPM = "RPM";
        private const string _seconds = "S";
        private const string _kiloWatts = "KW";
        private const string _watts = "W";
        private const string _volts = "V";
        private const string _yards = "Yards";

        public UnitItem(Units units, UnitType type, double scale, double shift)
        {
            if (scale < double.Epsilon)
            {
                Telemetry.TrackException(new ArgumentException("Scale factor cannot be zero."));
                throw new ArgumentException("Scale factor cannot be zero.");
            }

            this.Type = type;
            this.Units = units;
            this.Scale = scale;
            this.Shift = shift;
        }

        public bool Equals(UnitItem a)
        {
            return (a.Units == this.Units) && (a.Type == this.Type);
        }

        static public string ToString(Units units)
        {
            switch (units)
            {
                case Units.AmpHrs:
                    return _ampHrs;

                case Units.Amps:
                    return _amps;

                case Units.Bar:
                    return _bar;

                case Units.CubicMeters:
                    return _cubicMeters;

                case Units.CubicMetersPerHr:
                    return _cubicMetersPerHour;

                case Units.Celsius:
                    return _degreeC;

                case Units.Date:
                    return _date;

                case Units.Deciliters:
                    return _deciliters;

                case Units.Degrees:
                    return _degrees;

                case Units.DegreesPerSecond:
                    return _degreesPerSecond;

                case Units.DegreesPerMinute:
                    return _degreesPerMinute;

                case Units.Fathoms:
                    return _fathoms;

                case Units.Fahrenheit:
                    return _degreeF;

                case Units.Feet:
                    return _feet;

                case Units.GallonsUK:
                    return _gallonsUK;

                case Units.GallonsUS:
                    return _gallonsUS;

                case Units.GallonsUSPerNauticalMile:
                    return _gallonsUSPerNauticalMile;

                case Units.GallonsUSPerHr:
                    return _gallonsUSPerHour;

                case Units.GallonsUSPerMinute:
                    return _gallonsUSPerMinute;

                case Units.GallonsUKPerNauticalMile:
                    return _gallonsUKPerNauticalMile;

                case Units.GallonsUKPerHr:
                    return _gallonsUKPerHour;

                case Units.GallonsUKPerMinute:
                    return _gallonsUKPerMinute;

                case Units.Hertz:
                    return _hertz;

                case Units.Hours:
                    return _hours;

                case Units.InchesHg:
                    return _inchesOfMercury;

                case Units.Kelvin:
                    return _degreeK;

                case Units.Kg:
                    return _kilograms;

                case Units.KgPerCubicMeter:
                    return _kilogramsPerCubicMeter;

                case Units.KgPerLiter:
                    return _kilogramsPerLiter;

                case Units.KgPerHour:
                    return _kilogramsPerHour;

                case Units.Km:
                    return _kilometers;

                case Units.KilometersPerHour:
                    return _kilometersPerHour;

                case Units.KiloWatts:
                    return _kiloWatts;

                case Units.Knots:
                    return _nauticalMilesPerHour;

                case Units.KPa:
                    return _kilopascal;

                case Units.LitersPerNauticalMile:
                    return _litersPerNauticalMile;

                case Units.Liters:
                    return _liters;

                case Units.LitersPerHr:
                    return _litersPerHour;

                case Units.LitersPerMinute:
                    return _litersPerMinute;

                case Units.Meters:
                    return _meters;

                case Units.MetersPerSecond:
                    return _metersPerSecond;

                case Units.MetricTons:
                    return _metricTons;

                case Units.Miles:
                    return _miles;

                case Units.Minutes:
                    return _minutes;

                case Units.MM:
                    return _millimetres;

                case Units.MmHg:
                    return _millimetreOfMercury;

                case Units.MilesPerHour:
                    return _milesPerHour;

                case Units.NauticalMiles:
                    return _nauticalMiles;

                case Units.Null:
                    return _null;

                case Units.OFF:
                    return _OFF;

                case Units.ON:
                    return _ON;

                case Units.ONOFF:
                    return _ONOFF;

                case Units.Other:
                    return "unknown";

                case Units.Pascals:
                    return _pascals;

                case Units.Percent:
                    return _percent;

                case Units.Pounds:
                    return _pounds;

                case Units.PoundsPerHour:
                    return _poundsPerHour;

                case Units.PSI:
                    return _PSI;

                case Units.Radians:
                    return _radians;

                case Units.RadiansPerSecond:
                    return _radiansPerSecond;

                case Units.RPM:
                    return _RPM;

                case Units.Seconds:
                    return _seconds;

                case Units.Volts:
                    return _volts;

                case Units.Watts:
                    return _watts;

                case Units.Yards:
                    return _yards;

                default:
                    return "known";
            }

        }

        public override string ToString()
        {
            switch (Units)
            {
                case Units.AmpHrs:
                    return _ampHrs;

                case Units.Amps:
                    return _amps;

                case Units.Bar:
                    return _bar;

                case Units.CubicMeters:
                    return _cubicMeters;

                case Units.CubicMetersPerHr:
                    return _cubicMetersPerHour;

                case Units.Celsius:
                    return _degreeC;

                case Units.Date:
                    return _date;

                case Units.Deciliters:
                    return _deciliters;

                case Units.Degrees:
                    return _degrees;

                case Units.DegreesPerSecond:
                    return _degreesPerSecond;

                case Units.DegreesPerMinute:
                    return _degreesPerMinute;

                case Units.Fathoms:
                    return _fathoms;

                case Units.Fahrenheit:
                    return _degreeF;

                case Units.Feet:
                    return _feet;

                case Units.GallonsUK:
                    return _gallonsUK;

                case Units.GallonsUS:
                    return _gallonsUS;

                case Units.GallonsUSPerNauticalMile:
                    return _gallonsUSPerNauticalMile;

                case Units.GallonsUSPerHr:
                    return _gallonsUSPerHour;

                case Units.GallonsUSPerMinute:
                    return _gallonsUSPerMinute;

                case Units.GallonsUKPerNauticalMile:
                    return _gallonsUKPerNauticalMile;

                case Units.GallonsUKPerHr:
                    return _gallonsUKPerHour;

                case Units.GallonsUKPerMinute:
                    return _gallonsUKPerMinute;

                case Units.Hertz:
                    return _hertz;

                case Units.Hours:
                    return _hours;

                case Units.InchesHg:
                    return _inchesOfMercury;

                case Units.Kelvin:
                    return _degreeK;

                case Units.Kg:
                    return _kilograms;

                case Units.KgPerCubicMeter:
                    return _kilogramsPerCubicMeter;

                case Units.KgPerLiter:
                    return _kilogramsPerLiter;

                case Units.KgPerHour:
                    return _kilogramsPerHour;

                case Units.Km:
                    return _kilometers;

                case Units.KilometersPerHour:
                    return _kilometersPerHour;

                case Units.KiloWatts:
                    return _kiloWatts;

                case Units.Knots:
                    return _nauticalMilesPerHour;

                case Units.KPa:
                    return _kilopascal;

                case Units.LitersPerNauticalMile:
                    return _litersPerNauticalMile;

                case Units.Liters:
                    return _liters;

                case Units.LitersPerHr:
                    return _litersPerHour;

                case Units.LitersPerMinute:
                    return _litersPerMinute;

                case Units.Meters:
                    return _meters;

                case Units.MetersPerSecond:
                    return _metersPerSecond;

                case Units.MetricTons:
                    return _metricTons;

                case Units.Miles:
                    return _miles;

                case Units.Minutes:
                    return _minutes;

                case Units.MM:
                    return _millimetres;

                case Units.MmHg:
                    return _millimetreOfMercury;

                case Units.MilesPerHour:
                    return _milesPerHour;

                case Units.NauticalMiles:
                    return _nauticalMiles;

                case Units.Null:
                    return _null;

                case Units.OFF:
                    return _OFF;

                case Units.ON:
                    return _ON;

                case Units.ONOFF:
                    return _ONOFF;

                case Units.Other:
                    return "unknown";

                case Units.Pascals:
                    return _pascals;

                case Units.Percent:
                    return _percent;

                case Units.Pounds:
                    return _pounds;

                case Units.PoundsPerHour:
                    return _poundsPerHour;

                case Units.PSI:
                    return _PSI;

                case Units.Radians:
                    return _radians;

                case Units.RadiansPerSecond:
                    return _radiansPerSecond;

                case Units.RPM:
                    return _RPM;

                case Units.Seconds:
                    return _seconds;

                case Units.Volts:
                    return _volts;

                case Units.Watts:
                    return _watts;

                case Units.Yards:
                    return _yards;

                default:
                    return "known";
            }
        }

    }

    public class UnitsConverter
    {
        static private Hashtable _completeList = new Hashtable();       // Use hashtable so that the search is order 1

        /// <summary>
        /// Follows is a list of supported unit converters. The first entry in each group is the 'base' entry
        /// and each subsequent entry is a conversion factor between the base and the given entry. For example, 
        /// in temperature, the base unit is Kelvin, and Fahernheit and Celsius are given as conversions to Kelvin.
        /// </summary>
        private static UnitItem Kelvin = new UnitItem(Units.Kelvin, UnitType.Temperature, 1, 0);
        private static UnitItem Fahrenheit = new UnitItem(Units.Fahrenheit, UnitType.Temperature, 1.8, -459.67);
        private static UnitItem Celsius = new UnitItem(Units.Celsius, UnitType.Temperature, 1, -273.15);

        private static UnitItem GallonsUS = new UnitItem(Units.GallonsUS, UnitType.Volume, 1, 0);
        private static UnitItem GallonsUK = new UnitItem(Units.GallonsUK, UnitType.Volume, 0.8326741846289889D, 0);
        private static UnitItem Deciliters = new UnitItem(Units.Deciliters, UnitType.Volume, 37.85411784D, 0);
        private static UnitItem Litres = new UnitItem(Units.Liters, UnitType.Volume, 3.785411784D, 0);
        private static UnitItem CubicMeters = new UnitItem(Units.Liters, UnitType.Volume, 0.003785411784D, 0);

        private static UnitItem Degrees = new UnitItem(Units.Degrees, UnitType.Angle, 57.29577951308233, 0);
        private static UnitItem Radians = new UnitItem(Units.Radians, UnitType.Angle, 1, 0);

        private static UnitItem Meters = new UnitItem(Units.Meters, UnitType.Length, 1, 0);
        private static UnitItem MilliMeters = new UnitItem(Units.MM, UnitType.Length, 1000, 0);
        private static UnitItem Feet = new UnitItem(Units.Feet, UnitType.Length, 3.280839895013123, 0);
        private static UnitItem Fathoms = new UnitItem(Units.Fathoms, UnitType.Length, 0.5468066491688539, 0);
        private static UnitItem NauticalMiles = new UnitItem(Units.NauticalMiles, UnitType.Length, 5.399568034557235e-4, 0);
        private static UnitItem Miles = new UnitItem(Units.Miles, UnitType.Length, 6.21371192237334e-4, 0);
        private static UnitItem Kilometers = new UnitItem(Units.Km, UnitType.Length, 0.001, 0);
        private static UnitItem Yards = new UnitItem(Units.Yards, UnitType.Length, 1.093613298337708, 0);

        private static UnitItem Amps = new UnitItem(Units.Amps, UnitType.Current, 1, 0);
        private static UnitItem Volts = new UnitItem(Units.Volts, UnitType.Voltage, 1, 0);
        private static UnitItem Hertz = new UnitItem(Units.Hertz, UnitType.Frequency, 1, 0);
        private static UnitItem Watts = new UnitItem(Units.Watts, UnitType.Power, 1, 0);
        private static UnitItem KiloWatts = new UnitItem(Units.KiloWatts, UnitType.Power, 1.0 / 1000.0, 0);

        private static UnitItem MetersPerSecond = new UnitItem(Units.MetersPerSecond, UnitType.Speed, 1, 0);
        private static UnitItem Knots = new UnitItem(Units.Knots, UnitType.Speed, 1.943844492440605, 0);
        private static UnitItem MilesPerHour = new UnitItem(Units.MilesPerHour, UnitType.Speed, 2.236936292054402, 0);
        private static UnitItem KilometersPerHour = new UnitItem(Units.KilometersPerHour, UnitType.Speed, 3.6, 0);

        private static UnitItem Pascals = new UnitItem(Units.Pascals, UnitType.Pressure, 1.0, 0);
        private static UnitItem KiloPascals = new UnitItem(Units.KPa, UnitType.Pressure, 1.0 / 1000.0, 0);
        private static UnitItem PSI = new UnitItem(Units.PSI, UnitType.Pressure, 1.450377438972831e-4, 0);
        private static UnitItem MmHg = new UnitItem(Units.MmHg, UnitType.Pressure, 0.0075018754688672, 0);
        private static UnitItem InchesHg = new UnitItem(Units.InchesHg, UnitType.Pressure, 2.953494279081575e-4, 0);
        private static UnitItem Bar = new UnitItem(Units.Bar, UnitType.Pressure, 0.00001, 0);

        private static UnitItem RPM = new UnitItem(Units.RPM, UnitType.Rotation, 1, 0);

        private static UnitItem CubicMetersPerHour = new UnitItem(Units.CubicMetersPerHr, UnitType.VolumeFlow, 1, 0);
        private static UnitItem GallonsUSPerHour = new UnitItem(Units.GallonsUSPerHr, UnitType.VolumeFlow, 264.1720523581484, 0);
        private static UnitItem GallonsUSPerMinute = new UnitItem(Units.GallonsUSPerMinute, UnitType.VolumeFlow, 264.1720523581484 / 60, 0);
        private static UnitItem GallonsUKPerHour = new UnitItem(Units.GallonsUKPerHr, UnitType.VolumeFlow, 219.9692482990878, 0);
        private static UnitItem GallonsUKPerMinute = new UnitItem(Units.GallonsUKPerMinute, UnitType.VolumeFlow, 219.9692482990878 / 60, 0);
        private static UnitItem LitersPerHour = new UnitItem(Units.LitersPerHr, UnitType.VolumeFlow, 1000.0, 0);
        private static UnitItem LitersPerMinute = new UnitItem(Units.LitersPerMinute, UnitType.VolumeFlow, 1000.0 / 60.0, 0);

        private static UnitItem Hours = new UnitItem(Units.Hours, UnitType.Time, 1.0 / (60.0 * 60.0), 0);
        private static UnitItem Minutes = new UnitItem(Units.Minutes, UnitType.Time, 1.0 / 60.0, 0);
        private static UnitItem Seconds = new UnitItem(Units.Seconds, UnitType.Time, 1.0, 0);

        private static UnitItem Percent = new UnitItem(Units.Percent, UnitType.Percent, 1, 0);

        private static UnitItem Date = new UnitItem(Units.Date, UnitType.Date, 1, 0);

        private static UnitItem RadiansPerSecond = new UnitItem(Units.RadiansPerSecond, UnitType.AngularRate, 1, 0);
        private static UnitItem DegreesPerSecond = new UnitItem(Units.DegreesPerSecond, UnitType.AngularRate, 57.29577951308233, 0);
        private static UnitItem DegreesPerMinute = new UnitItem(Units.DegreesPerMinute, UnitType.AngularRate, 57.29577951308233 / 60D, 0);

        private static UnitItem GallonsUSPerNauticalMile = new UnitItem(Units.GallonsUSPerNauticalMile, UnitType.Mileage, 1, 0);
        private static UnitItem LitersPerNauticalMile = new UnitItem(Units.LitersPerNauticalMile, UnitType.Mileage, 3.785411784, 0);
        private static UnitItem LitersPerKilometer = new UnitItem(Units.LitersPerKilometer, UnitType.Mileage, 1.852, 0);

        private static UnitItem Off = new UnitItem(Units.OFF, UnitType.State, 1, 0);
        private static UnitItem On = new UnitItem(Units.ON, UnitType.State, 1, 0);
        private static UnitItem OnOff = new UnitItem(Units.ONOFF, UnitType.State, 1, 0);

        private static UnitItem AmpHours = new UnitItem(Units.AmpHrs, UnitType.ElectricCharge, 1, 0);

        private static UnitItem NullUnits = new UnitItem(Units.Null, UnitType.Null, 1, 0);

        private static UnitItem Kilograms = new UnitItem(Units.Kg, UnitType.Mass, 1, 0);
        private static UnitItem MetricTons = new UnitItem(Units.MetricTons, UnitType.Mass, 0.001D, 0);

        private static UnitItem KilogramsPerCubicMeter = new UnitItem(Units.KgPerCubicMeter, UnitType.Density, 1, 0);
        private static UnitItem KilogramsPerLiter = new UnitItem(Units.KgPerLiter, UnitType.Density, 0.001, 0);

        /// <summary>
        /// Build all of supported units (gallons, celcius, PSI, etc).
        /// </summary>
        static UnitsConverter()
        {
            _completeList.Add(Units.Celsius, Celsius);
            _completeList.Add(Units.Fahrenheit, Fahrenheit);
            _completeList.Add(Units.Kelvin, Kelvin);

            _completeList.Add(Units.GallonsUS, GallonsUS);
            _completeList.Add(Units.GallonsUK, GallonsUK);
            _completeList.Add(Units.Liters, Litres);
            _completeList.Add(Units.Deciliters, Deciliters);
            _completeList.Add(Units.CubicMeters, CubicMeters);

            _completeList.Add(Units.Degrees, Degrees);
            _completeList.Add(Units.Radians, Radians);

            _completeList.Add(Units.Meters, Meters);
            _completeList.Add(Units.MM, MilliMeters);
            _completeList.Add(Units.Feet, Feet);
            _completeList.Add(Units.Fathoms, Fathoms);
            _completeList.Add(Units.NauticalMiles, NauticalMiles);
            _completeList.Add(Units.Miles, Miles);
            _completeList.Add(Units.Km, Kilometers);
            _completeList.Add(Units.Yards, Yards);

            _completeList.Add(Units.Amps, Amps);
            _completeList.Add(Units.Volts, Volts);
            _completeList.Add(Units.Hertz, Hertz);
            _completeList.Add(Units.Watts, Watts);
            _completeList.Add(Units.KiloWatts, KiloWatts);

            _completeList.Add(Units.Knots, Knots);
            _completeList.Add(Units.MetersPerSecond, MetersPerSecond);
            _completeList.Add(Units.MilesPerHour, MilesPerHour);
            _completeList.Add(Units.KilometersPerHour, KilometersPerHour);

            _completeList.Add(Units.Pascals, Pascals);
            _completeList.Add(Units.KPa, KiloPascals);
            _completeList.Add(Units.PSI, PSI);
            _completeList.Add(Units.MmHg, MmHg);
            _completeList.Add(Units.InchesHg, InchesHg);
            _completeList.Add(Units.Bar, Bar);

            _completeList.Add(Units.RPM, RPM);

            _completeList.Add(Units.CubicMetersPerHr, CubicMetersPerHour);
            _completeList.Add(Units.GallonsUSPerHr, GallonsUSPerHour);
            _completeList.Add(Units.GallonsUSPerMinute, GallonsUSPerMinute);
            _completeList.Add(Units.GallonsUKPerHr, GallonsUKPerHour);
            _completeList.Add(Units.GallonsUKPerMinute, GallonsUKPerMinute);
            _completeList.Add(Units.LitersPerHr, LitersPerHour);
            _completeList.Add(Units.LitersPerMinute, LitersPerMinute);

            _completeList.Add(Units.Hours, Hours);
            _completeList.Add(Units.Minutes, Minutes);
            _completeList.Add(Units.Seconds, Seconds);

            _completeList.Add(Units.Percent, Percent);

            _completeList.Add(Units.Date, Date);

            _completeList.Add(Units.RadiansPerSecond, RadiansPerSecond);
            _completeList.Add(Units.DegreesPerSecond, DegreesPerSecond);
            _completeList.Add(Units.DegreesPerMinute, DegreesPerMinute);

            _completeList.Add(Units.GallonsUSPerNauticalMile, GallonsUSPerNauticalMile);
            _completeList.Add(Units.LitersPerNauticalMile, LitersPerNauticalMile);
            _completeList.Add(Units.LitersPerKilometer, LitersPerKilometer);

            _completeList.Add(Units.ON, On);
            _completeList.Add(Units.OFF, Off);
            _completeList.Add(Units.ONOFF, OnOff);

            _completeList.Add(Units.AmpHrs, AmpHours);

            _completeList.Add(Units.Other, new UnitItem(Units.Other, UnitType.Uninitialized, 1, 0));

            _completeList.Add(Units.Null, new UnitItem(Units.Null, UnitType.Null, 1, 0));

            _completeList.Add(Units.Kg, Kilograms);
            _completeList.Add(Units.MetricTons, MetricTons);

            _completeList.Add(Units.KgPerCubicMeter, KilogramsPerCubicMeter);
            _completeList.Add(Units.KgPerLiter, KilogramsPerLiter);
        }

        /// <summary>
        /// Used to convert between two units of the same type (type==volume, speed, pressure, etc). If the types
        /// are different (try to convert between pressure and volume) this procedure will throw and exception/.
        /// </summary>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <param name="fromValue"></param>
        /// <returns></returns>
        public static double Convert(UnitItem fromUnit, UnitItem toUnit, double fromValue)
        {
            if (fromUnit.Type != toUnit.Type)
            {
                string message = string.Format("Conversion between incompatible types. (from: '{0}' to '{1}')",
                                                fromUnit.Type,
                                                toUnit.Type);

                Telemetry.TrackException(new InvalidOperationException(message));
                throw new InvalidOperationException(message);
            }

            if (fromUnit.Type == UnitType.Uninitialized) return fromValue;

            double v = (fromValue - fromUnit.Shift) / fromUnit.Scale;
            v = (v * toUnit.Scale) + toUnit.Shift;
            return v;
        }

        /// <summary>
        /// Find the specific units requested (gallons, meters, PSI, etc)
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static UnitItem Find(Units unit)
        {
            return (UnitItem)_completeList[unit];
        }

        /// <summary>
        /// Find all of the units of a specific type (distance, volume, pressure, etc).
        /// </summary>
        /// <param name="myType"></param>
        /// <returns></returns>
        public static List<UnitItem> FindAll(UnitType myType)
        {
            List<UnitItem> result = new List<UnitItem>();
            foreach (DictionaryEntry item in _completeList)
            {
                if (((UnitItem)item.Value).Type == myType) result.Add((UnitItem)item.Value);
            }

            return result;
        }
    }
}
