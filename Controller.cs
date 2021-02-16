using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeomSolve
{
    public class Controller
    {
        public List<GeomType> geomTypes;
        public List<Geometry> geoms;

        public Controller()
        {
            geoms = new List<Geometry>();
            geomTypes = new List<GeomType>();

            LoadGeomTypes();
        }

        private void LoadGeomTypes()
        {
            //Loading the XML data files
        }

        public void CreateGeom(GeomType type, string name, bool isGiven)
        {
            geoms.Add(new Geometry(this, type, name, isGiven));
        }

        public GeomType GetTypeByString(string type)
        {
            foreach (GeomType geomType in geomTypes)
            {
                if (geomType.type == type)
                    return geomType;
            }

            throw new InvalidOperationException($"The geom type {type} is not loaded in the program (or doesn't exist in the .xml files)!");
        }

        public Geometry GetGeometryByType(string type)
        {
            foreach (Geometry geom in geoms)
            {
                if (geom.geomType.type == type)
                    return geom;
            }

            return null;
        }

        public Geometry GetGeometryByName(string name)
        {
            foreach(Geometry geom in geoms)
            {
                if (geom.name == name)
                    return geom;
            }
            
            return null;
        }
        
    }
}
