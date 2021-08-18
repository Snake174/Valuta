using Models;

namespace Interfaces
{
    interface IDataBase
    {
        string GetValIdByName(string valName);
        void SaveCurse(ValCurse vc);
        void SaveInfo(ValInfo vi);
        decimal GetValCurse(string val, string date);
    }
}
