//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////   

using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Utilities;
using System;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface ISensorItem
    {
        void AddOfflineObservation(bool forceFlush);
        void AddOfflineObservation(DateTime timeUTC, bool forceFlush);
        void AddSensorValue(DateTime timeUTC, double value, bool isOnline, bool forceFlush);
        void AddSensorValue(double value, bool isOnline, bool forceFlush);
        Task BeginCommit();
        Task BeginDelete();
        DateTime ChangeDate { get; }
        bool IsDemoMode { get; set; }
        string Description { get; set; }
        Int64 DeviceId { get; set; }
        void DisableSensorDataCache();
        void EmptySensorDataCache();
        void EnableSensorDataCache();
        string FriendlyName { get; }
        double HighAlarmValue { get; set; }
        double HighWarningValue { get; set; }
        bool IsCalibrated { get; set; }
        bool IsDirty { get; }
        bool IsEnabled { get; set; }
        bool IsHighAlarmEnabled { get; set; }
        bool IsHighWarningEnabled { get; set; }
        bool IsLowAlarmEnabled { get; set; }
        bool IsLowWarningEnabled { get; set; }
        bool IsOnline { get; set; }
        bool IsPersisted { get; set; }
        bool IsVirtual { get; set; }
        string Location { get; set; }
        double LowAlarmValue { get; set; }
        double LowWarningValue { get; set; }
        double MaxValue { get; set; }
        double MinValue { get; set; }
        string Name { get; set; }
        double NominalValue { get; set; }
        bool PersistDataPoints { get; set; }
        uint PGN { get; set; }
        double PercentFull { get; }
        int PortNumber { get; set; }
        int Priority { get; set; }
        int Resolution { get; set; }
        void Rollback();
        Int64 SensorId { get; }
        SensorType SensorType { get; set; }
        UnitType SensorUnitType { get; set; }
        Units SensorUnits { get; set; }
        SensorUsage SensorUsage { get; set; }
        double SensorValue { get; }
        string SerialNumber { get; set; }
        bool ShowNominalValue { get; set; }
        TimeSpan Throttle { get; set; }
        DateTime Time { get; }
    }

    public enum SensorType : int
    {
        Unknown = 00,
        AC = 01,                    // AC Volts, Current, Power, Frequency
        Amps = 02,                  // Current (DC or AC)
        Angle = 03,                 // Angle (Rudder, Latitude, Longtitude, Thruster, etc)
        Battery = 04,               // Batteries
        Charge = 05,                // Electrical Charge (AmpHours)
        CourseSpeed = 06,           // Course & Speed over Ground (obsolete, replaced by Direction & Speed)
        CurrentDirection = 07,      // Direction of the water current (a virtual sensor)
        CurrentSpeed = 08,          // Speed of the water current (a virtual sensor)
        DC_Amps = 09,               // DC Current
        Depth = 10,                 // Water Depth
        Distance = 11,              // Length (Anchor Chain, Centerboard Extension, etc)
        Flow = 12,                  // Flow Rate
        FlowTotal = 13,             // Total Flow of a group of flow sensors
        Frequency = 14,             // AC Frequency
        FuelEfficiency = 15,        // Fuel Efficiency (Gal / NM)
        Heading = 16,               // Heading
        MagneticVariation = 17,     // Magnetic Variation
        Percent = 18,               // Percentage (Engine Load, etc)
        Position = 19,              // GPS Position (obsolete, replaced by Latitude & Longtitude)
        Power = 20,                 // Electrical Power (AC or DC)
        PowerTotal = 21,            // Total power of a group of power sensors
        Pressure = 22,              // Oil, Fuel, Boost Pressure, etc
        Rotation = 23,              // Things that rotate (Engine RPM, Propellor Shaft, etc)
        Speed = 24,                 // Speed (water, wind, etc)
        String = 25,                // Strings that cannot be stored as doubles
        Switch = 26,                // Any other generic binary value switch
        Tank = 27,                  // Tank Senders
        TankTotal = 28,             // Total Tank of a group of tank sensors
        Temperature = 29,           // Temperature
        Text = 30,                  // Generic text string
        Time = 31,                  // Clocks, Hour Meters, etc
        VideoCamera = 32,           // Video camera
        Volts = 33,                 // Volts (DC or AC)
        Volume = 34,                // Volume of Fluid or Gas
        VolumeResettable = 35,      // Resettable volume
        VolumeTotal = 36,           // Total Volume of Fluid or Gas
        VolumeTotalResettable = 0x37,   // Resettable volume total
        Wind = 38                   // Wind Data (obsolete, replaced by direction & speed)
    }

    public enum SensorUsage : int
    {
        Other = 0,                              // Other = not specified

        //
        //Amps Sensor Usage
        //
        AmpsACOther = 10,
        AmpsACShore = 11,           // AC Shore Amps
        AmpsACGenerator = 12,           // AC Generator Amps
        AmpsACInverter = 13,           // AC Inverter Amps
        AmpsACBuss = 14,           // AC Buss Amps
        AmpsACSentinel = 15,           // Must be last AC

        AmpsDCOther = 30,           // Other DC Amps
        AmpsDCBattery = 31,           // House Battery Amps
        AmpsDCSrcPropeller = 32,           // DC Current Source Propeller (A)
        AmpsDCLoad = 33,           // DC Current Load Sensor (A)
        AmpsDCBuss = 34,           // DC Buss amps
        AmpsDCSentinel = 35,           // Must be last DC amps

        //
        //Angle Sensor Usage
        //
        AngleOther = 50,
        AngleHeading = 51,
        AngleCOG = 52,
        AngleWindDirection = 53,
        AngleLatitude = 54,
        AngleLongitude = 55,
        AngleCurrentDirection = 56,
        AngleRudder = 57,
        AngleAutoPilotHeading = 58,
        AngleWaypointLatitude = 59,
        AngleWaypointLongtitude = 60,
        AngleMagneticVariation = 61,
        AnglePitch = 62,
        AngleRoll = 63,
        AngleYaw = 64,
        AngleXTE = 65,
        AngleBearingToWayPoint = 66,
        AngleSentinel = 67,       // Must be last angle

        //
        //Battery Charge
        //
        ChargeBattery = 80,

        //
        // Distance Usage
        //
        DistanceOther = 100,
        DistanceAnchorChain = 101,
        DistanceXTE = 102,
        DistanceDepth = 103,
        DistanceAltitude = 104,
        DistanceSentinel = 105,      // Must be last distance

        //
        //Flow Sensor Usage
        //
        FlowOther = 120,     // Other Flows we have not thought of
        FlowFuel = 121,     // Fuel Flow Rate
        FlowWater = 122,     // Water Flow Rate
        FlowSentinel = 123,     // Must be last flow

        //
        //Frequency Sensor Usage
        //
        FrequencyOther = 140,
        FrequencyACShore = 141,     // AC Shore Frequency
        FrequencyACGenerator = 142,     // AC Generator Frequency
        FrequencyACInverter = 143,     // AC Inverter Frequency
        FrequencyACBuss = 144,     // AC Buss Frequency
        FrequencyACSentinel = 145,     // Must be last AC frequency

        //
        // Mileage Usage
        //
        MileageOther = 160,
        MileageSentinel = 161,      // Must be last mileage

        //
        //Percent Sensor Usage
        //
        PercentOther = 180,      // Other Percentage
        PercentEngineLoad = 181,      // Engine Load
        PercentTorque = 182,      // Engine Torque
        PercentHumidityOther = 183,      // Other kinds of humidity
        PercentHumidityInside = 184,      // Inside Humidity
        PercentHumidityOutside = 185,      // Outside Humidity
        PercentPower = 186,      // Percent of Power
        PercentEngineTiltTrim = 187,      // Range 0 - 100%, where 0% =Full Down (trim) and 100% = Full Up (tilt) Positions
        PercentSentinel = 188,      // Must be last percent

        //
        //Pressure Sensor Usage
        //
        PressureOther = 200,      // Other Pressures we have not thought of
        PressureEngineOil = 201,      // Engine Oil Pressure
        PressureEngineCoolant = 202,      // Engine Coolant Pressure
        PressureEngineFuel = 203,      // Engine Fuel Pressure
        PressureEngineBoost = 204,      // Engine Turbocharger Boost Pressure
        PressureTransmissionOil = 205,      // Transmission Oil Pressure
        PressureAir = 206,      // Air pressure
        PressureWater = 207,      // Water pressure
        PressureSteam = 208,      // Steam pressure
        PressureCompressedAir = 209,      // Compressed air pressure
        PressureHydraulic = 210,      // Hyrdaulic pressure
        PressureSentinel = 211,      // Must be last pressure

        //
        //Rotation Sensor Usage
        //
        RotationOther = 230,      // Other rotating machinery
        RotationEngine = 231,      // Engine Speed
        RotationPropellor = 232,      // Propellor Speed
        RotationGenset = 233,      // Generator rotation
        RotationRateOfTurn = 234,      // Rate of turn
        RotationMotor = 235,      // Motor Speed
        RotationSentinel = 236,      // Must be last rotation

        //
        // Speed Sensor Usage
        //
        SpeedOther = 250,      // Other kinds of speeds
        SpeedThruWater = 251,      // Speed through the water
        SpeedOverGround = 252,      // Speed referenced to Ground
        SpeedWind = 253,      // Wind speed
        SpeedWatercurrent = 254,      // Current speed
        SpeedSentinel = 255,      // Must be last speed

        // Various strings
        StringOther = 270,
        StringVIN = 271,
        StringSentinel = 272,

        //
        //Tank Sensor Usage
        //
        TanksOther = 290,      // Tank Sensor (Gallons)
        TanksWaterFresh = 291,      // Tank Sensor (Gallons)
        TanksWaterGrey = 292,      // Tank Sensor (Gallons)
        TanksWaterBlack = 293,      // Tank Sensor (Gallons)
        TanksFuel = 294,      // Tank Sensor (Gallons)
        TanksFuelDay = 295,      // Day Fuel Tank (Gallons)
        TanksHydraulic = 296,      // Hydraulic Fluid
        TanksOil = 297,      // Oil
        TanksWaterBallast = 298,      // Ballast Water
        TanksWaterCoolant = 299,      // Coolant Water
        TanksOilLube = 300,      // Used Oil
        TanksLivewell = 301,      // Livewell
        TanksSentinel = 302,      // Must be last tanks

        //
        //Temperature Sensor Usage
        //
        TempOther = 320,   // Other kinds of temperature
        TempAir = 321,   // Air Temperature
        TempFluid = 322,   // Fluid Temperature
        TempHydraulicFluid = 323,   // Hydraulic Fluid Temperature
        TempEngineOil = 324,   // Engine Oil Temperature
        TempTransmissionOil = 325,   // Transmission Oil Temperature
        TempEngineCoolant = 326,   // Coolant Temperature
        TempSeaWater = 327,   // Sea Water Temperature
        TempOutsideAir = 328,   // Outside Air Temperature
        TempInsideAir = 329,   // Inside Air Temperature
        TempEngineRoomAir = 320,   // Engine Room Air Temperature
        TempCabinRoomAir = 331,   // Cabin Air Temperature
        TempLiveWell = 332,   // Live well temperature
        TempBaitWell = 333,   // Bait well temperature
        TempRefrigeration = 334,   // Refrigeration temperature
        TempHeatingSystem = 335,   // Heating System temperature
        TempDewPoint = 336,   // Dew Point temperature
        TempFreezer = 337,   // Freezer temperature
        TempExhaustGas = 338,   // Exhaust Gas temperature
        TempFuel = 339,   // Fuel temperature
        TempBatteryCase = 340,   // Battery case temperature
        TempSentinel = 341,   // Must be last temperature

        //
        // Time Sensor Usage
        //
        TimeOther = 360,      // Other Time Values we have not thought of
        TimeEngineHours = 361,      // Engine Hours
        TimeClockDate = 362,      //
        TimeClockTime = 363,      //
        TimeSentinel = 364,      // Must be last time

        //
        // Volume Usage
        //
        VolumeOther = 380,
        VolumeFuelUsed = 381,
        VolumeTotalFuelUsedVoyage = 382,    // Fuel used on a particular voyage
        VolumeFuelUsedVoyage = 383,
        VolumeFuelUsedPeriodicReset = 384,
        VolumeSentinel = 385,      // Must be last volume

        //
        //Volts Sensor Usage
        //
        VoltsACOther = 400,
        VoltsACShore = 401,      // AC Shore Volts
        VoltsACGenerator = 402,      // AC Generator Volts
        VoltsACInverter = 403,      // AC Inverter Volts
        VoltsACBuss = 404,      // AC Buss volts
        VoltsACSentinel = 405,      // Must be last AC volts

        VoltsDCOther = 420,
        VoltsDCBattery = 421,      // DC Battery Volts
        VoltsDCAlternator = 422,      // Engine Alternator Volts
        VoltsDCBuss = 423,      // DC Buss volts
        VoltsDCSentinel = 424,      // Must be last DC volts

        //
        //Power Sensor Usage
        //
        WattsOther = 440,
        WattsACShore = 441,      // AC Shore Watts
        WattsACGenerator = 442,      // AC Generator Watts
        WattsACInverter = 443,      // AC Inverter Watts
        WattsACBuss = 444,      // AC Buss
        WattsACSentinel = 445,      // Must be last AC watts
    }

}
