using System;
using Interfaces;

namespace Val
{
    class DataBaseFactory
    {
        public DataBaseFactory()
        {
        }

        public IDataBase GetDataBase()
        {
            INI ini = new INI("settings.ini");
            string connection = ini.Read("current", "Connection");

            Type type = Type.GetType($"DataBases.{connection}DataBase");
            return (IDataBase)Activator.CreateInstance(type);
        }
    }
}
