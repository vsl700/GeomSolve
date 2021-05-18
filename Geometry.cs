using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomSolve
{
    public class Geometry
    {
        public Controller controller;
        
        public string name;
        public GeomType geomType;
        public Geometry[] parentGeoms;
        public Value[] values;
        public bool isGiven; //Whether this geom is auto-created, or written by the user in the task

        public List<string> args;//This will hold special (or proven) data, for example if this is a line, whether it is in parallel with another line


        public Geometry(Controller controller, GeomType geomType, string name, bool isGiven)
        {
            this.controller = controller;
            this.geomType = geomType;
            this.name = name;
            this.isGiven = isGiven;
            args = new List<string>();

            geomType.typeAmount++;

            CreateValues();

            if (ParentGeomTypes.Length > 0)
            {
                string[] dependencies = geomType.nameDependencies.Split(';');
                if(dependencies.Length != geomType.parentGeoms.Length)
                {
                    throw new InvalidOperationException($"Name dependencies amount and parent geoms amount are not equal for the {geomType.type}: parents: {geomType.parentGeoms}, dependencies: {dependencies.Length}");
                }

                //Search for existing parent geoms according to the name and if there're no existing ones, create som
                for(int i = 0; i < dependencies.Length; i++)
                {
                    string[] depParts = dependencies[i].Split('='); //This array should always appear with two elements

                    if(depParts.Length != 2)
                    {
                        throw new Exception($"Name dependency {i} in {geomType.type} document: the amount of = symbols can be only one!");
                    }

                    depParts[0] = Operations.DecodeString(this, depParts[0]);
                    depParts[1] = Operations.DecodeString(this, depParts[1]);

                    if (AreGeomNamesEqual(depParts[0], depParts[1]))
                    {
                        parentGeoms[i] = controller.GetGeometryByName(depParts[0]);
                    }
                    else
                    {
                        controller.CreateGeom(controller.GetTypeByString(ParentGeomTypes[i]), depParts[0], false);
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether two geom names are mathematically equal. 
        /// 
        /// THIS METHOD SHOULD NOT BE USED FOR THE NAMES OF DIFFERENT TYPES OF GEOMS!
        /// </summary>
        /// <param name="geomName1"></param>
        /// <param name="geomName2"></param>
        /// <returns></returns>
        private bool AreGeomNamesEqual(string geomName1, string geomName2)
        {
            if (geomName1.Length != geomName2.Length)
                return false;

            if (geomType.canReplaceNameWords)
            {
                foreach (char c in geomName1)
                {
                    if (!geomName2.Contains(c))
                        return false;
                }

                return true;
            }
            else
            {
                return geomName1 == geomName2 || new string(geomName1.Reverse().ToArray()) == geomName2;
            }
        }

        /// <summary>
        /// Checks whether this and the given geom's names are mathematically equal. 
        /// 
        /// THIS METHOD SHOULD NOT BE USED FOR THE NAME OF A DIFFERENT TYPE OF GEOM!
        /// </summary>
        /// <param name="geomName1"></param>
        /// <param name="geomName2"></param>
        /// <returns></returns>
        public bool IsGeomNameEqualToOther(Geometry geometry)
        {
            if (!geomType.Equals(geometry.geomType))
                throw new InvalidOperationException("The argument geom's type is not equal to this geom's one");

            return AreGeomNamesEqual(name, geometry.name);
        }

        private void CreateValues()
        {
            values = new Value[geomType.valueDatas.Length];

            for(int i = 0; i < values.Length; i++)
            {
                values[i] = new Value(this, Operations.DecodeString(this, geomType.valueDatas[i].valueName), geomType.valueDatas[i].valueDesc, geomType.valueDatas[i].valueGeomType);
            }
        }

        public bool IterateThroughRules()
        {
            foreach(string rule in geomType.geomRules)
            {
                //Think about implementing the Solver!
            }

            return false;
        }

        public override string ToString()
        {
            return Operations.DecodeString(this, geomType.nameSpec);
        }

        public string[] ParentGeomTypes { get { return geomType.parentGeoms; } }

        public class Value
        {
            private Geometry ownerGeom;

            public string valueName;//AH, AB, etc.
            public string valueDesc;//a, h, radius, etc.
            public GeomType valueType;
            public List<Value> linkedValues; //Two triangles might have one side together (ABC and ABC1 for example will have AB)

            private double value;

            public Value(Geometry ownerGeom, string name, string desc, string type)
            {
                this.ownerGeom = ownerGeom;

                valueName = name;
                valueDesc = desc;

                if(type != null)
                    valueType = ownerGeom.controller.GetTypeByString(type);

                linkedValues = new List<Value>();
            }

            public double GValue//I didn't use operator methods because the code is just shorter here, and also we use a value linking value variable, which will not allow operator methods!
            {
                get
                {
                    if (linkedValues.Count > 0)
                        return linkedValues[0].GValue;//Linked values have the same values

                    return value;
                }

                set
                {
                    if(valueType != null)
                    {
                        if (linkedValues.Count == 0)
                        {
                            Geometry tempGeom = ownerGeom.controller.GetGeometryByName(valueName);
                            if (tempGeom != null)
                            {
                                tempGeom.values[0].LinkValue(this);//Assuming that the main value will be at index 0
                            }
                            else
                            {
                                //Values will correct their names according to the future geoms renderer! Renderer should check the geoms every time a change is made before rendering them and correct them
                                ownerGeom.controller.CreateGeom(valueType, valueName, false);
                            }
                        }

                        foreach (var linkVal in linkedValues)
                            linkVal.GValue = value;//Setting all the linked values to a given decimal number
                    }
                    else
                        this.value = value;
                }
            }

            public void LinkValue(Value value)
            {
                linkedValues.Add(value);
                value.linkedValues.Add(this);
            }
        }
    }
}
