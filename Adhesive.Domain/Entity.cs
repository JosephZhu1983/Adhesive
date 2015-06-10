using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Adhesive.Domain
{
    [DataContract]
    public abstract class Entity : IIdentifiable
    {
        #region Members

        int? _requestedHashCode;
        string _id;

        #endregion
        #region Properties
        [Key]
        [StringLength(32)]
        [DataMember]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnIdChanged();
            }
        }
        #endregion
        #region Abstract Methods

        /// <summary>
        /// When POID is changed
        /// </summary>
        protected virtual void OnIdChanged()
        {

        }
        #endregion
        #region Public Methods
        public Entity()
        {
            _id = Guid.Empty.ToString("N");
        }
        /// <summary>
        /// Check if this entity is transient, ie, without identity at this moment
        /// </summary>
        /// <returns>True if entity is transient, else false</returns>
        public bool IsTransient()
        {
            return this.Id == Guid.Empty.ToString("N");
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// <see cref="M:System.Object.Equals"/>
        /// </summary>
        /// <param name="obj"><see cref="M:System.Object.Equals"/></param>
        /// <returns><see cref="M:System.Object.Equals"/></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        /// <summary>
        /// <see cref="M:System.Object.GetHashCode"/>
        /// </summary>
        /// <returns><see cref="M:System.Object.GetHashCode"/></returns>
        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        #endregion
    }
}
