using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeomSolve
{
    [XmlType(TypeName = "geomtype")]
    public abstract class GeomType
    {
        public int typeAmount;//How many geoms of this type are there in the system currently

        [XmlAttribute(AttributeName = "type")]
        public string type;

        [XmlAttribute(AttributeName = "nameDep")]
        public string nameDependencies;//A triangle's name depends on its three parent lines, a circle's name is like k(parentPoint, radius)

        [XmlAttribute(AttributeName = "nameSpec")]
        public string nameSpec;//For example angle ABC will be shown as ∠ABC

        [XmlAttribute(AttributeName = "nameReplace")]
        public bool canReplaceNameWords;//Whether triangle ABC = BAC, or angle BAC = BCA (IT'S NOT WHETHER ANGLE ABC = CBA, REVERSE DOESN'T MATTER)

        [XmlArray("parents")]
        [XmlArrayItem("parent")]
        public string[] parentGeoms; //For example, a 'triangle' is made of 'lines', 'lines' of 'points', etc.

        [XmlArray("rules")]
        [XmlArrayItem("rule")]
        public string[] geomRules;//The commands that will run during exercise solving!

        [XmlArray("values")]
        [XmlArrayItem("value")]
        public ValueData[] valueDatas;

        [XmlType(TypeName = "value")]
        public class ValueData
        {
            [XmlElement(ElementName = "valueName")]
            public string valueName; //name(0), name(1), etc. for using the geom's name when writing XML code!!!

            [XmlElement(ElementName = "valueDesc")]
            public string valueDesc;

            [XmlElement(ElementName = "valueGeomType")]
            public string valueGeomType;//Value AB will be a type of 'line', value angle ABC will be type of 'angle', null type will mean it's not gonna be connected to a value from another geom

        }
    }
}
