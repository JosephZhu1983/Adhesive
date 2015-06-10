
using System;
using System.Xml.Serialization;

namespace Adhesive.Common
{
    /// <summary>
    /// 可以序列化的TimeSpan
    /// </summary>
    public struct TimeSpanEx
    {
        #region Constructor

        private TimeSpan _timeSpan;

        public TimeSpanEx(int hours, int minutes, int seconds)
        {
            _timeSpan = new TimeSpan(hours, minutes, seconds);

        }

        public TimeSpanEx(TimeSpan timeSpan)
        {
            _timeSpan = new TimeSpan();
            this._timeSpan = timeSpan;
        }

        #endregion

        [XmlText]
        public string XmlText
        {
            get
            {
                return _timeSpan.ToString();
            }
            set
            {
                TimeSpan.TryParse(value, out _timeSpan);
            }
        }

        #region Convertor

        // User-defined conversion from TimeSpanEx to TimeSpan
        public static implicit operator TimeSpan(TimeSpanEx t)
        {
            return t._timeSpan;
        }
        //  User-defined conversion from TimeSpan to TimeSpanEx 
        public static implicit operator TimeSpanEx(TimeSpan t)
        {
            return new TimeSpanEx(t);
        }

        #endregion

        #region Override
        public override string ToString()
        {
            return _timeSpan.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            if (this.GetType() != obj.GetType())
                return false;
            TimeSpanEx timeSpanEx = (TimeSpanEx)obj;
            if (_timeSpan.Equals(timeSpanEx._timeSpan))
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return _timeSpan.GetHashCode();
        }
        #endregion
    }
}
