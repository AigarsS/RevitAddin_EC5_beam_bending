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
        [ValueWithName(Name = "@s{m,y,d}", Index = 5)]
        public Double sigmaMyd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Moment, DisplayUnit = DisplayUnitType.DUT_NEWTON_METERS)]
        [ValueWithName(Name = "M{Ed}", Index = 6)]
        public Double MEd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Stress, DisplayUnit = DisplayUnitType.DUT_PASCALS)]
        [ValueWithName(Name = "f{m,d}", Index = 7)]
        public Double fmd { get; set; }

        [SchemaProperty(Unit = Autodesk.Revit.DB.UnitType.UT_Section_Modulus, DisplayUnit = DisplayUnitType.DUT_CUBIC_CENTIMETERS)]
        [ValueWithName(Name = "W{y}", Index = 2)]
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
        [Ratio(Index = 8)]
        public Double Ratio { get; set; }

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

