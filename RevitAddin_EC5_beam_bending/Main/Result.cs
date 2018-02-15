using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage.Framework.Attributes;
using Autodesk.Revit.DB.ExtensibleStorage.Framework.Documentation.Attributes;

namespace RevitAddin_EC5_beam_bending
{
    [Autodesk.Revit.DB.ExtensibleStorage.Framework.Attributes.Schema("Result", "bbe5870b-bb46-4bfd-a272-ec43beaf395a")]
    public class Result : Autodesk.Revit.DB.ExtensibleStorage.Framework.SchemaClass
    {
        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "@s{m,y,d}", Index = 6)]
        public Double sigmaMyd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Moment, DisplayUnit = DisplayUnitType.DUT_NEWTON_METERS)]
        [ValueWithName(Name = "M{y,Ed}", Index = 5)]
        public Double MyEd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "@s{m,z,d}", Index = 8)]
        public Double sigmaMzd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Moment, DisplayUnit = DisplayUnitType.DUT_NEWTON_METERS)]
        [ValueWithName(Name = "M{z,Ed}", Index = 7)]
        public Double MzEd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "@s{c,0,d}", Index = 10)]
        public Double sigmaC0d { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Force, DisplayUnit = DisplayUnitType.DUT_NEWTONS)]
        [ValueWithName(Name = "N{Ed}", Index = 9)]
        public Double NEd { get; set; }



        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "f{m,d}", Index = 11)]
        public Double fmd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Section_Modulus, DisplayUnit = DisplayUnitType.DUT_CUBIC_CENTIMETERS)]
        [ValueWithName(Name = "W{z}", Index = 2)]
        public Double Wz { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Section_Modulus, DisplayUnit = DisplayUnitType.DUT_CUBIC_CENTIMETERS)]
        [ValueWithName(Name = "W{y}", Index = 1)]
        public Double Wy { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Section_Area, DisplayUnit = DisplayUnitType.DUT_SQUARE_METERS)]
        [ValueWithName(Index = 0)]
        public Double A { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "f{m,k}", Index = 3)]
        public Double fmk { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "f{c,0,k}", Index = 4)]
        public Double fc0k { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Number, DisplayUnit = DisplayUnitType.DUT_1_RATIO)]
        [Ratio(Index = 12)]
        public Double Ratio1 { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Number, DisplayUnit = DisplayUnitType.DUT_1_RATIO)]
        [Ratio(Index = 13)]
        public Double Ratio2 { get; set; }


        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Force, DisplayUnit = DisplayUnitType.DUT_NEWTONS)]
        [ValueWithName(Name = "V{z,Ed}", Index = 14)]
        public Double Vzd { get; set; }


        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "f{v,0,k}", Index = 15)]
        public Double fv0k { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "f{v,d}", Index = 16)]
        public Double fvd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Number, DisplayUnit = DisplayUnitType.DUT_1_RATIO)]
        [Ratio(Index = 17)]
        public Double Ratio3 { get; set; }



        public Result()
        {
        }

        public Result(Document document)
        {
        }

        public Result(Autodesk.Revit.DB.ExtensibleStorage.Entity entity, Document document)
            : base(entity, document)
        {
        }
    }
}

