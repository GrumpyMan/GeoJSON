﻿using BAMCIS.GeoJSON.Serde;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BAMCIS.GeoJSON
{
    /// <summary>
    /// Feature objects in GeoJSON contain a Geometry object and additional members.
    /// </summary>
    [JsonConverter(typeof(InheritanceBlockerConverter))]
    public class Feature : GeoJson
    {
        #region Public Properties

        /// <summary>
        /// The geometry of this feature
        /// </summary>
        [JsonProperty(PropertyName = "geometry")]
        public Geometry Geometry { get; }

        /// <summary>
        /// Additional properties for the feature, the value
        /// is dynamic as it could be a string, bool, null, number,
        /// array, or object
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public IDictionary<string, dynamic> Properties { get; }

        /// <summary>
        ///Unique id feature, the value
        /// is dynamic as it could be a string, null, number
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public dynamic Id { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Builds a new Feature with the given geometry and optional properties
        /// </summary>
        /// <param name="geometry">The geometry to create the feature from</param>
        /// <param name="properties">The feature properties</param>
        [JsonConstructor]
        public Feature(Geometry geometry, IDictionary<string, dynamic> properties = null, IEnumerable<double> boundingBox = null, dynamic id = null) : base(GeoJsonType.Feature, geometry == null ? false : geometry.IsThreeDimensional(), boundingBox)
        {
            this.Geometry = geometry; // Geometry can be null
            this.Properties = properties ?? new Dictionary<string, dynamic>();
            this.Id = id;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a Feature from json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static new Feature FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Feature>(json);
        }

        /// <summary>
        /// Compares equality of this Feature to another. It is important to note
        /// that only the Keys at the top level of the properties dictionary are
        /// checked to determine equality of that property.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            Feature Other = (Feature)obj;

            bool BBoxEqual = true;

            if (this.BoundingBox != null && Other.BoundingBox != null)
            {
                BBoxEqual = this.BoundingBox.SequenceEqual(Other.BoundingBox);
            }
            else
            {
                BBoxEqual = (this.BoundingBox == null && Other.BoundingBox == null);
            }

            bool PropertiesEqual = true;

            if (this.Properties != null && Other.Properties != null)
            {
                PropertiesEqual = this.Properties.Keys.SequenceEqual(Other.Properties.Keys);
            }
            else
            {
                PropertiesEqual = (this.Properties == null && Other.Properties == null);
            }

            bool IdEqual = true;

            if (this.Id != null && Other.Id != null)
            {
                if (this.Id.GetType() == Other.Id.GetType())
                {
                    IdEqual = this.Id == Other.Id;
                }
                else
                {
                    IdEqual = false;
                }
            }
            else
            {
                IdEqual = (this.Id == null && Other.Id == null);
            }


            return this.Type == Other.Type &&
                   this.Geometry == Other.Geometry &&
                   BBoxEqual &&
                   PropertiesEqual &&
                   IdEqual;
        }

        public override int GetHashCode()
        {
            return Hashing.Hash(this.Type, this.Geometry, this.BoundingBox, this.Properties, this.Id);
        }

        public static bool operator ==(Feature left, Feature right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (right is null || left is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Feature left, Feature right)
        {
            return !(left == right);
        }

        #endregion
    }
}
