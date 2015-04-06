using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace COM.SingNo.XNLCore
{
    /// <summary>
    /// 定义xnl标签里的参数类型
    /// </summary>
    public class XNLParam
    {
        private string _name;
        private XNLType _type=XNLType.Object;
        private object _val;
        private DbType _dbType=DbType.Object;
        public string name
        {
            get
            {
                if (_name == null) return string.Empty;
                return _name;
            }
            set
            {
                _name = value;
            }

        }

        public XNLType type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                try
                {
                    _dbType = getDBTypeFromTypeStr(getTypeName(_type));
                }
                catch { }                
            }

        }

        public DbType dbType
        {
            get
            {

                return _dbType;
            }
            //set
            //{

            //    _dbType = value;
            //}

        }

        public object value
        {
            get
            {
                if (_val == null) return string.Empty;
                return _val;
            }
            set
            {
                _val = value;
            }

        }

        public XNLParam()
        {
        }
        public XNLParam(object t_thevalue)
        {
            _val = t_thevalue;
        }
        public XNLParam(XNLType t_type)
        {
            type = t_type;
        }
        public XNLParam(XNLType t_type, object t_thevalue)
        {
            type = t_type;
            _val = t_thevalue;
        }
        public XNLParam(string t_name, object t_thevalue, XNLType t_type)
        {
            _name = t_name;
            _val = t_thevalue;
            type = t_type;
        }
        public static Type _type_XNLType =typeof(XNLType);  //Type.GetType("COM.SingNo.Core.XNLType", false, false);
        public static Type _type_DBType =typeof(DbType);// Type.GetType("System.Data.DbType", false, false);
        public static string getTypeName(XNLType _type)
        {
            return Enum.GetName(_type_XNLType, _type);
        }
        public static XNLType getTypeFromTypeStr(string _typeStr)
        {
            XNLType _type = XNLType.Object;
            try
            {
                _type = (XNLType)Enum.Parse(_type_XNLType, _typeStr, true);
            }catch{

            }
            return _type;
        }
        public static DbType getDBTypeFromTypeStr(string _typeStr)
        {
            DbType _type = DbType.Object;
            try
            {
                _type = (DbType)Enum.Parse(_type_DBType, _typeStr, true);
            }
            catch{}
            return _type;
        }
        public static DbType getDBTypeFromTypeId(int typeId)
        {
            DbType _type = DbType.Object;
            try
            {
                _type = (DbType)Enum.ToObject(_type_DBType,typeId);
            }
            catch{}
            return _type;
        }
    }
}