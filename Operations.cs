using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace GeomSolve
{
    /*class Operation<T>
    {
        public T Act(params object[] args) { throw new NotImplementedException($"An operation returning {typeof(T)} does not have an implemented method!"); }
    }*/

    delegate string stringOperation(Geometry geometry, params object[] args);
    delegate Geometry geomOperation(Geometry geometry, params object[] args);
    public class Operations
    {
        //private static string[] operations = new string[] { "symb", "parent", "name" };
        private static Dictionary<string, stringOperation> stringOperations = new Dictionary<string, stringOperation>() { { "name", NAME }, { "value", VALUE } };
        private static Dictionary<string, geomOperation> geomOperations = new Dictionary<string, geomOperation>() { { "parent", PARENT } };

        /*public static string SYMB(Geometry geometry, int index)
        {
            if (index < -1)
                throw new ArgumentOutOfRangeException($"symb({index}) is an invalid argument (less than zero!)!");

            return geometry.name[index] + "";
        }*/

        public static string VALUE(Geometry geometry, params object[] args)
        {
            int index = (int) args[0];

            if (geometry.values[index].GValue == 0)//If there's no value assigned to the current value
            {
                if (geometry.geomType.typeAmount > 1 || geometry.values[index].valueDesc == "")//If there's for example two or more triangles or if the value has no description (like the lines)
                    return geometry.values[index].valueName;


                return geometry.values[index].valueDesc;
            }

            return geometry.values[index].GValue.ToString();
        }

        public static Geometry PARENT(Geometry geometry, params object[] args)
        {
            int index = (int)args[0];

            if (index < 0)
                throw new ArgumentOutOfRangeException($"parent({index}) in the {geometry.geomType.type} file is an invalid argument (less than zero!)!");

            if (index > geometry.name.Length)
                throw new ArgumentOutOfRangeException($"parent({index}) in the {geometry.geomType.type} file is an invalid argument (more than the geom's parent geoms!)!");

            return geometry.parentGeoms[index];
        }

        public static string NAME(Geometry geometry, params object[] args)
        {
            int index = (int)args[0];

            if (index < 0)
                throw new ArgumentOutOfRangeException($"name({index}) in the {geometry.geomType.type} file is an invalid argument (less than zero!)!");
            
            if (index > geometry.name.Length)
                throw new ArgumentOutOfRangeException($"name({index}) in the {geometry.geomType.type} file is an invalid argument (bigger than the geom's name!)!");
            
            if (index < -1)
                return geometry.name;
            
            return geometry.name[index] + "";
        }

        public static string DecodeString(Geometry geometry, string command)
        {
            string returnStr = "";
            int commandLength;
            for(int i = 0; i < command.Length; i+= commandLength)
            {
                if (geometry == null)
                    return null;//The PARENT operation changes the argument geometry value to a geom's parent

                commandLength = 1;

                if (command[i] == '/')
                    continue;

                string tempCommand = "";
                foreach (string str in stringOperations.Keys)
                {
                    if(command.IndexOf(str, i) == i)
                    {
                        commandLength = str.Length;//Used as a flag
                        tempCommand = str;
                        break;
                    }
                }

                foreach (string str in geomOperations.Keys)
                {
                    if (command.IndexOf(str, i) == i)
                    {
                        commandLength = str.Length;//Used as a flag
                        tempCommand = str;
                        break;
                    }
                }

                if (commandLength > 1)
                {
                    if(command[i + commandLength] == '(')
                    {
                        int indexOfClose = commandLength = command.IndexOf(')');
                        int argNum = int.Parse(command.Substring(command[i + commandLength + 1], indexOfClose - (i + commandLength)));

                        if (stringOperations.ContainsKey(tempCommand))
                            returnStr += stringOperations[tempCommand](geometry, argNum);
                        else geometry = geomOperations[tempCommand](geometry, argNum);
                    }
                }
                else 
                { 
                    returnStr += command[i];
                }
            }

            return returnStr;
        }

    }
}
