using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace TransactionalNodeService
{
    public class MapParameters : IPersistableSessionContainer
    {
        protected const string SelectParametersFromSessionId = "SELECT * FROM [Parameters] WHERE [SessionUid] = @SessionId;";

        protected Dictionary<Guid, MapParameter> _parameterContainer;

        protected MapParameters()
        {
        }

        public MapParameters(IMapObjects mapObjects)
            : this()
        {
            MapObjects = mapObjects;
        }

        protected IMapObjects MapObjects
        {
            get;
            set;
        }

        protected Dictionary<Guid, MapParameter> ParameterContainer
        {
            get
            {
                if (_parameterContainer == null)
                {
                    _parameterContainer = new Dictionary<Guid, MapParameter>();
                }

                return _parameterContainer;
            }
        }

        public void AddParameter(MapParameter parameter)
        {
            if (parameter != null)
            {
                ParameterContainer[parameter.Id] = parameter;
            }
        }

        public MapParameter AddParameter(MapParameterType parameterType, Guid parameterValue, bool isDelayed)
        {
            MapParameter newParameter = new MapParameter();
            newParameter.Value = parameterValue;
            newParameter.SessionId = MapObjects.Session.Id;
            newParameter.IsDelayed = isDelayed;
            newParameter.ParameterType = parameterType;

            AddParameter(newParameter);

            return newParameter;
        }

        public MapParameter AddParameter(Guid parameterId, MapParameterType parameterType, Guid parameterValue, bool isDelayed)
        {
            MapParameter newParameter = new MapParameter(parameterId);
            newParameter.Value = parameterValue;
            newParameter.SessionId = MapObjects.Session.Id;
            newParameter.IsDelayed = isDelayed;
            newParameter.ParameterType = parameterType;

            AddParameter(newParameter);

            return newParameter;
        }

        public MapParameter this[Guid parameterId]
        {
            get
            {
                if (ParameterContainer.ContainsKey(parameterId))
                {
                    return ParameterContainer[parameterId];
                }
                else
                {
                    SqlParameter parameterIdSqlParameter = new SqlParameter("@ParameterId", parameterId);

                    SqlCommand getParameter = new SqlCommand("SELECT * FROM [Parameters] WHERE [ParameterUid] = @ParameterId;", MapObjects.ParametersDbConnection.Connection);
                    getParameter.Parameters.Add(parameterIdSqlParameter);

                    MapObjects.ParametersDbConnection.Open();

                    IDataReader parameters = getParameter.ExecuteReader();

                    MapParameter parameter = null;

                    while (parameters.Read())
                    {
                        parameter = new MapParameter();

                        parameter.LoadSessionObject(parameters);

                        AddParameter(parameter);
                    }

                    MapObjects.ParametersDbConnection.Close();

                    return parameter;
                }
            }
        }

        public void PersistSessionObject()
        {
            foreach (MapParameter parameter in ParameterContainer.Values)
            {
                parameter.PersistSessionObject(MapObjects.ParametersDbConnection);
            }
        }

        public void LoadParameters()
        {
            SqlParameter sqlSessionParameter = new SqlParameter("@SessionId", MapObjects.Session.Id);

            SqlCommand selectParametersFromId = new SqlCommand(SelectParametersFromSessionId, MapObjects.ParametersDbConnection.Connection);
            selectParametersFromId.Parameters.Add(sqlSessionParameter);

            MapObjects.ParametersDbConnection.Open();

            IDataReader parameters = selectParametersFromId.ExecuteReader();

            while (parameters.Read())
            {
                MapParameter parameter = new MapParameter();

                parameter.LoadSessionObject(parameters);

                AddParameter(parameter);
            }

            MapObjects.ParametersDbConnection.Close();
        }
    }
}