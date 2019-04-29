using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConventionMobile.Model
{
    public partial class GlobalOption
    {
        [PrimaryKey]
        public string ID { get; set; }

        public string type { get; set; }

        public string data_string { get; set; }
        public long data_long { get; set; }
        public bool data_bool { get; set; }
        public int data_int { get; set; }

        [Ignore]
        public object originalValue
        {
            get
            {
                if (!isOriginalValueSet)
                {
                    if (type == "System.String")
                    {
                        return this.data_string;
                    }
                    else if (type == "System.Boolean")
                    {
                        return this.data_bool;
                    }
                    else if (type == "System.Int32")
                    {
                        return this.data_int;
                    }
                    else if (type == "System.DateTime")
                    {
                        return new DateTime(this.data_long);
                    }
                }
                else if (_originalValue == null)
                {
                    return null;
                }
                else
                {
                    return _originalValue;
                }

                return null;
            }
            set
            {
                _originalValue = value;
                isOriginalValueSet = true;
            }
        }

        private bool isOriginalValueSet = false; 

        private object _originalValue { get; set; } = null;

        public GlobalOption(string ID, object value)
        {
            this.ID = ID;
            this.setValue(value);
            this.originalValue = value;
        }

        public GlobalOption()
        {

        }

        public void setValue(object value)
        {
            Type t = value.GetType();
            this.type = t.FullName;

            if (t == typeof(string))
            {
                this.data_string = ((string)value).Replace("\\r","\r").Replace("\\n","\n").Replace("\\\\r", "\r").Replace("\\\\n", "\n");
            }
            else if (t == typeof(String))
            {
                this.data_string = ((string)value).Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\\\r", "\r").Replace("\\\\n", "\n");
            }
            else if (t == typeof(bool))
            {
                this.data_bool = (bool)value;
            }
            else if (t == typeof(int))
            {
                this.data_int = (int)value;
            }
            else if (t == typeof(DateTime))
            {
                this.data_long = ((DateTime)value).Ticks;
            }
        }

        public T getData<T>()
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)getStringData();
            }
            else if (typeof(T) == typeof(String))
            {
                return (T)(object)getStringData();
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)(object)getBoolData();
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)getIntData();
            }
            else if (typeof(T) == typeof(long))
            {
                return (T)(object)getLongData();
            }
            else if (typeof(T) == typeof(DateTime))
            {
                return (T)(object)getDateTimeData();
            }
            return default(T);
        }

        
        public string getStringData()
        {
            try
            {
                return this.data_string;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public bool getBoolData()
        {
            try
            {
                return this.data_bool;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DateTime getDateTimeData()
        {
            try
            {
                return new DateTime(this.data_long);
            }
            catch (Exception)
            {
                return new DateTime();
            }
        }

        public int getIntData()
        {
            try
            {
                return this.data_int;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public long getLongData()
        {
            try
            {
                return this.data_long;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
