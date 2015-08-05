using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.BusinessData.Runtime;
using Microsoft.BusinessData.MetadataModel;
using Microsoft.BusinessData.MetadataModel.Collections;
using Microsoft.Office.Server.Search.Connector;
using Microsoft.Office.Server.Search.Connector.BDC;
using Microsoft.SharePoint.Administration;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Binds to a GlymaRepositoryProxy class for accessing a Glyma repository and executes methods by invoking the proxy using Reflection.
   /// </summary>
   public class GlymaRepositoryUtility : StructuredRepositorySystemUtility<GlymaRepositoryProxy>
   {
      private readonly object _globalSettingsLock = new object();
      private bool _globalSettingsInitialised = false;
      private DataAccessType _dataAccessType = DataAccessType.Sql;
      private string _securityConnectionString = string.Empty;
      private NodeCrawlRules _crawlRules = new NodeCrawlRules();
      private bool _isNodeAclCacheEnabled = true;
      private NodeAclType _nodeAclType = NodeAclType.Windows;
      public bool _isSecurityEnabled = true;
      private TimeSpan _nodeAclCacheDuration = new TimeSpan(0, 10, 0);
      private TimeSpan _nodeAclCacheAutoExpirePeriod = BasicCacheConstants.DefaultAutoExpirePeriod;
      private int _nodeAclCacheMaxItems = BasicCacheConstants.DefaultMaxItems;
      private TimeSpan _nodeAclTaskWaitDuration = new TimeSpan(0, 0, 60);


      protected override GlymaRepositoryProxy CreateProxy()
      {
         return new GlymaRepositoryProxy();
      }


      protected override void DisposeProxy(GlymaRepositoryProxy proxy)
      {
         proxy.Dispose();
      }


      protected override void SetConnectionString(GlymaRepositoryProxy proxy, string connectionString)
      {
         proxy.StartAddress = connectionString;
      }


      protected override void SetCertificates(GlymaRepositoryProxy proxy, System.Security.Cryptography.X509Certificates.X509CertificateCollection certifcates)
      {
         throw new NotImplementedException();
      }


      protected override void SetCookies(GlymaRepositoryProxy proxy, System.Net.CookieContainer cookies)
      {
         throw new NotImplementedException();
      }


      protected override void SetCredentials(GlymaRepositoryProxy proxy, string userName, string passWord)
      {
         throw new NotImplementedException();
      }


      protected override void SetProxyServerInfo(GlymaRepositoryProxy proxy, string proxyServerName, string bypassList, bool bypassProxyForLocalAddress)
      {
         throw new NotImplementedException();
      }


      protected T ParseEnum<T>(string value)
      {
         T enumValue;

         enumValue = (T)Enum.Parse(typeof(T), value, true);

         if (!Enum.IsDefined(typeof(T), enumValue))
         {
            throw new ArgumentException(value + " is not an underlying value of the " + typeof(T).Name + " enumeration.");
         }

         return enumValue;
      }


      protected virtual void GetDataAccessType(INamedPropertyDictionary properties)
      {
         lock (_globalSettingsLock)
         {
            if (!properties.ContainsKey(GlymaModelConstants.DataAccessType) || string.IsNullOrEmpty((string)properties[GlymaModelConstants.DataAccessType]))
            {
               throw new ArgumentException("The property: " + GlymaModelConstants.DataAccessType + ", has not been specified in the model.");
            }

            string dataAccessTypeString = (string)properties[GlymaModelConstants.DataAccessType];
            DataAccessType dataAccessType;
            try
            {
               dataAccessType = ParseEnum<DataAccessType>(dataAccessTypeString);
            }
            catch (ArgumentException)
            {
               throw new ArgumentException("The property: " + GlymaModelConstants.DataAccessType + ", has an invalid value.  The allowed values are: (" + string.Join(", ", Enum.GetNames(typeof(DataAccessType))) + ").");
            }
           
            _dataAccessType = dataAccessType;
         }
      }


      protected virtual void GetSecurityConnectionString(INamedPropertyDictionary properties)
      {
         lock (_globalSettingsLock)
         {
            if (!properties.ContainsKey(GlymaModelConstants.SecurityConnectionString) || string.IsNullOrEmpty((string)properties[GlymaModelConstants.SecurityConnectionString]))
            {
               throw new ArgumentException("A security repository connection string has not been specified in the model.");
            }
            _securityConnectionString = (string)properties[GlymaModelConstants.SecurityConnectionString];
         }
      }


      protected virtual void GetEnableNodeAclCacheSetting(INamedPropertyDictionary properties)
      {
         lock (_globalSettingsLock)
         {
            if (properties.ContainsKey(GlymaModelConstants.EnableNodeAclCache))
            {
               _isNodeAclCacheEnabled = (bool)properties[GlymaModelConstants.EnableNodeAclCache];
            }
         }
      }


      protected virtual void GetNodeAclType(INamedPropertyDictionary properties)
      {
         lock (_globalSettingsLock)
         {
            if (!properties.ContainsKey(GlymaModelConstants.NodeAclType) || string.IsNullOrEmpty((string)properties[GlymaModelConstants.NodeAclType]))
            {
               throw new ArgumentException("The property: " + GlymaModelConstants.NodeAclType + ", has not been specified in the model.");
            }

            string nodeAclTypeString = (string)properties[GlymaModelConstants.NodeAclType];
            NodeAclType nodeAclType;
            try
            {
               nodeAclType = ParseEnum<NodeAclType>(nodeAclTypeString); 
            }
            catch (ArgumentException)
            {
               throw new ArgumentException("The property: " + GlymaModelConstants.NodeAclType + ", has an invalid value.  The allowed values are: (" + string.Join(", ", Enum.GetNames(typeof(NodeAclType))) + ").");
            }

            _nodeAclType = nodeAclType;
         }
      }


      protected virtual void GetEnableSecuritySetting(INamedPropertyDictionary properties)
      {
         lock (_globalSettingsLock)
         {
            if (properties.ContainsKey(GlymaModelConstants.EnableSecurity))
            {
               _isSecurityEnabled = (bool)properties[GlymaModelConstants.EnableSecurity];
            }
         }
      }


      protected virtual void GetNodeAclCacheDuration(INamedPropertyDictionary properties)
      {
         lock (_globalSettingsLock)
         {
            if (properties.ContainsKey(GlymaModelConstants.NodeAclCacheDuration))
            {
               int cacheDuration = (int)properties[GlymaModelConstants.NodeAclCacheDuration];
               if (cacheDuration < 0)
               {
                  throw new ArgumentException("The propery: " + GlymaModelConstants.NodeAclCacheDuration + ", cannot be a negative number.");
               }
               _nodeAclCacheDuration = new TimeSpan(0, 0, (int)properties[GlymaModelConstants.NodeAclCacheDuration]);
            }
         }
      }


      protected virtual void GetNodeAclCacheAutoExpirePeriod(INamedPropertyDictionary properties)
      {
         lock (_globalSettingsLock)
         {
            if (properties.ContainsKey(GlymaModelConstants.NodeAclCacheAutoExpirePeriod))
            {
               int cacheAutoExpirePeriod = (int)properties[GlymaModelConstants.NodeAclCacheAutoExpirePeriod];
               if (cacheAutoExpirePeriod < 0)
               {
                  throw new ArgumentException("The propery: " + GlymaModelConstants.NodeAclCacheAutoExpirePeriod + ", cannot be a negative number.");
               }
               _nodeAclCacheAutoExpirePeriod = new TimeSpan(0, 0, (int)properties[GlymaModelConstants.NodeAclCacheAutoExpirePeriod]);
            }
         }
      }


      protected virtual void GetNodeAclCacheMaxItems(INamedPropertyDictionary properties)
      {
         lock (_globalSettingsLock)
         {
            if (properties.ContainsKey(GlymaModelConstants.NodeAclCacheMaxItems))
            {
               int maxItems = (int)properties[GlymaModelConstants.NodeAclCacheMaxItems];
               if (maxItems < 0)
               {
                  throw new ArgumentException("The propery: " + GlymaModelConstants.NodeAclCacheMaxItems + ", cannot be a negative number.");
               }
               _nodeAclCacheMaxItems = maxItems;
            }
         }
      }


      protected virtual void GetNodeAclTaskWaitDuration(INamedPropertyDictionary properties)
      {
         lock (_globalSettingsLock)
         {
            if (properties.ContainsKey(GlymaModelConstants.NodeAclTaskWaitDuration))
            {
               int aclWaitDuration = (int)properties[GlymaModelConstants.NodeAclTaskWaitDuration];
               if (aclWaitDuration < 0)
               {
                  throw new ArgumentException("The propery: " + GlymaModelConstants.NodeAclTaskWaitDuration + ", cannot be a negative number.");
               }
               _nodeAclTaskWaitDuration = new TimeSpan(0, 0, (int)properties[GlymaModelConstants.NodeAclTaskWaitDuration]);
            }
         }
      }


      protected virtual void GetCrawlRules(INamedPropertyDictionary properties)
      {
         lock (_globalSettingsLock)
         {
            // Crawl rule order is important e.g. filter rules will usually be applied before additional post-processing rules, so loop through properties and
            // add crawl rules in the order that they are listed in the model.
            foreach (string propertyKey in properties.Keys)
            {
               if (propertyKey.Equals(CrawlRuleTypes.ExcludeSimpleQuestions, StringComparison.OrdinalIgnoreCase))
               {
                  SimpleQuestionFilter filter = new SimpleQuestionFilter();
                  _crawlRules.Add(filter);
               }
               else if (propertyKey.Equals(CrawlRuleTypes.ExcludeSpecifiedQuestions, StringComparison.OrdinalIgnoreCase))
               {
                  SpecifiedQuestionFilter filter = new SpecifiedQuestionFilter((string)properties[CrawlRuleTypes.ExcludeSpecifiedQuestions]);
                  _crawlRules.Add(filter);
               }
               else if (propertyKey.Equals(CrawlRuleTypes.IncludeParents, StringComparison.OrdinalIgnoreCase))
               {
                  IncludeParentsTransform transform = new IncludeParentsTransform();
                  _crawlRules.Add(transform);
               }
               else if (propertyKey.Equals(CrawlRuleTypes.IncludeChildren, StringComparison.OrdinalIgnoreCase))
               {
                  IncludeChildrenTransform transform = new IncludeChildrenTransform();
                  _crawlRules.Add(transform);
               }
            }
         }
      }


      public void SetGlobalSettings(GlymaRepositoryProxy proxy, INamedPropertyDictionary properties)
      {
         // The global settings will be accessed by multiple threads, so synchronise access to them when configuring a proxy. 
         lock (_globalSettingsLock)
         {
            if (!_globalSettingsInitialised)
            {
               GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Medium, "Initialised global settings.");
               GetDataAccessType(properties);
               GetSecurityConnectionString(properties);
               GetCrawlRules(properties);
               GetEnableNodeAclCacheSetting(properties);
               GetNodeAclType(properties);
               GetEnableSecuritySetting(properties);
               GetNodeAclCacheDuration(properties);
               GetNodeAclCacheAutoExpirePeriod(properties);
               GetNodeAclCacheMaxItems(properties);
               GetNodeAclTaskWaitDuration(properties);
               _globalSettingsInitialised = true;
            }

            IGlymaSecurityRepository securityRepository = null;
            if (_dataAccessType == DataAccessType.Sql)
            {
               securityRepository = new SqlGlymaSecurityRepository(_securityConnectionString);
            }

            GlymaSecurityManager securityManager = null;
            if (_nodeAclType == NodeAclType.Windows)
            {
               if (_isNodeAclCacheEnabled)
               {
                  securityManager = new WindowsGlymaSecurityManager(securityRepository, _nodeAclCacheAutoExpirePeriod, _nodeAclCacheMaxItems, _nodeAclCacheDuration, _nodeAclTaskWaitDuration);
               }
               else
               {
                  securityManager = new WindowsGlymaSecurityManager(securityRepository);
               }
            }
            else
            {
               throw new ApplicationException("Only the Windows node ACL type is currently supported.");
            }

            proxy.IsSecurityEnabled = _isSecurityEnabled;
            proxy.SecurityRepository = securityRepository;
            proxy.SecurityManager = securityManager;
            proxy.CrawlRules = _crawlRules.DeepCopy(); 
         }
      }


      protected virtual void SetStartAddress(GlymaRepositoryProxy proxy, string startAddress)
      {
         proxy.StartAddress = startAddress;
      }


      protected virtual void SetRepositoryName(GlymaRepositoryProxy proxy, string repositoryName)
      {
         proxy.RepositoryName = repositoryName;
      }


      protected virtual void SetMapConnectionString(GlymaRepositoryProxy proxy, INamedPropertyDictionary properties, string repositoryName)
      {
         string connectionString = string.Empty;
         if (!properties.ContainsKey(GlymaModelConstants.MapConnectionString) || string.IsNullOrEmpty((string)properties[GlymaModelConstants.MapConnectionString]))
         {
            throw new ArgumentException("A map repository connection string has not been specified in the model for the LOB instance: " + repositoryName + ".");
         }
         connectionString = (string)properties[GlymaModelConstants.MapConnectionString];

         if (_dataAccessType == DataAccessType.Sql)
         {
            proxy.MapRepository = new SqlGlymaMapRepository(connectionString);
         }
      }


      /// <summary>
      /// Executes a MethodInstance in the model against the specified external system instance with given parameters.
      /// </summary>
      /// <param name="methodInstance">The method instance being executed.</param>
      /// <param name="lobSystemInstance">The external system instance which the method instance is being executed against.</param>
      /// <param name="args">Parameters of the method. The size of the parameter array is equal to the number of parameter objects in the method definition in the BCS model file, and the values are passed in order. Out and return parameters will be a null reference.</param>
      /// <param name="context">ExecutionContext in which this execution is happening. This value will be a null reference if ExecutionContext is not created.</param>
      /// <remarks>
      /// This method is executed in a separate thread.
      /// </remarks>
      public override void ExecuteStatic(Microsoft.BusinessData.MetadataModel.IMethodInstance methodInstance, Microsoft.BusinessData.MetadataModel.ILobSystemInstance lobSystemInstance, object[] args, Microsoft.BusinessData.Runtime.IExecutionContext context)
      {
         // Modify the execution behaviour for the method instance types that are used in the Glyma repository model and revert to the base class implementation
         // for all other method instance types (until a better understanding is developed on how the other method instance types are used).
         switch (methodInstance.MethodInstanceType)
         {
            case MethodInstanceType.Finder:
            case MethodInstanceType.SpecificFinder:
            case MethodInstanceType.AssociationNavigator:
            case MethodInstanceType.ChangedIdEnumerator:
            case MethodInstanceType.DeletedIdEnumerator:
            case MethodInstanceType.BinarySecurityDescriptorAccessor:

               // Validate parameters to ensure all required execution details are available.
               if (methodInstance == null)
               {
                  throw new ArgumentNullException("methodInstance");
               }

               if (lobSystemInstance == null)
               {
                  throw new ArgumentNullException("lobSystemInstance");
               }

               if (args == null)
               {
                  throw new ArgumentNullException("args");
               }

               if (context == null)
               {
                  throw new ArgumentNullException("context");
               }

               IConnectionContext connectionContext = (IConnectionContext)context["ConnectionContext"];
               if (connectionContext == null)
               {
                  throw new ArgumentNullException("The BDC execution context doesn't contain a connection context.");
               }

               // Create the proxy and execute the method.
               GlymaRepositoryProxy proxy = null;
               try
               {
                  proxy = CreateProxy();
                  if (proxy == null)
                  {
                     throw new ArgumentException("The proxy returned from the BDC shim is null.");
                  }

                  GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Verbose, "Configuring proxy to execute " + methodInstance.MethodInstanceType.ToString() + " method for: " + connectionContext.Path.OriginalString);
                  SetGlobalSettings(proxy, lobSystemInstance.GetLobSystem().GetProperties());
                  SetStartAddress(proxy, connectionContext.Path.OriginalString);
                  SetRepositoryName(proxy, lobSystemInstance.Name);
                  SetMapConnectionString(proxy, lobSystemInstance.GetProperties(), lobSystemInstance.Name);

                  // Get the definition of the fields for the entity that will be returned by the proxy method.  This enables the connector to build entities
                  // based on the definitions in the model file instead of having entities with fixed pre-defined properties.      
                  ITypeDescriptorCollection entityFields = GetEntityFields(methodInstance);

                  GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Verbose, "Executing " + methodInstance.MethodInstanceType.ToString() + " proxy method for: " + connectionContext.Path.OriginalString);
                  ExecuteProxyMethod(proxy, methodInstance, args, entityFields);
                  GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Verbose, "Completed execution of " + methodInstance.MethodInstanceType.ToString() + " proxy method for: " + connectionContext.Path.OriginalString);
               }
               catch (Exception currentException)
               {
                  GlymaSearchLogger.WriteTrace(LogCategoryId.Connector, TraceSeverity.Unexpected, currentException.ToString());
                  throw;
               }
               finally
               {
                  if (proxy != null)
                  {
                     DisposeProxy(proxy);
                     proxy = null;
                  }
               }
               break;
            default:
               base.ExecuteStatic(methodInstance, lobSystemInstance, args, context);
               break;
         }
      }


      /// <summary>
      /// Get the fields that have been defined for the return entity of a method.
      /// </summary>
      /// <param name="methodInstance">The method to examine.</param>
      /// <returns>A collection of fields defined on the return entity of the the method.</returns>
      protected virtual ITypeDescriptorCollection GetEntityFields(Microsoft.BusinessData.MetadataModel.IMethodInstance methodInstance)
      {
         ITypeDescriptor returnType = methodInstance.GetReturnTypeDescriptor();
         ITypeDescriptorCollection entityFields = null;
         if (returnType.IsCollection)
         {
            ITypeDescriptorCollection childTypes = returnType.GetChildTypeDescriptors();
            if (childTypes.Count > 0)
            {
               entityFields = childTypes[0].GetChildTypeDescriptors();
            }
         }
         else
         {
            entityFields = returnType.GetChildTypeDescriptors();
         }

         return entityFields;
      }


      /// <summary>
      /// Executes the proxy method using reflection.
      /// </summary>
      /// <param name="proxy">A GlymaRepositoryProxy object used to execute the method.</param>
      /// <param name="methodInstance">The method to execute.</param>
      /// <param name="methodArgs">The parameters of the method.</param>
      /// <param name="entityFields">A ITypeDescriptorCollection object that contains the fields to retrieve for the entity containing the method to execute.</param>
      protected virtual void ExecuteProxyMethod(GlymaRepositoryProxy proxy, IMethodInstance methodInstance, object[] methodArgs, ITypeDescriptorCollection entityFields)
      {
         // Extract the method details required to execute it using reflection.
         IMethod method = methodInstance.GetMethod();
         string methodName = (method.LobName != null) ? method.LobName : method.Name;
         List<Type> parameterTypes = new List<Type>();
         List<object> parameters = new List<object>(methodArgs.Length);
         IParameterCollection parameterDefinitions = method.GetParameters();
         foreach (IParameter parameterDefinition in parameterDefinitions)
         {
            if (parameterDefinition.Direction != DirectionType.Return)
            {
               Type parameterType = parameterDefinition.TypeReflector.ResolveDotNetType(parameterDefinition.GetRootTypeDescriptor().TypeName, method.GetDataClass().GetLobSystem());
               if (parameterDefinition.Direction != DirectionType.In)
               {
                  parameterType = parameterType.MakeByRefType();
               }
               parameters.Add(methodArgs[parameterDefinition.OrdinalNumber]);
               parameterTypes.Add(parameterType);
            }
         }

         // Pass the entity fields as the last parameter to the proxy method.
         parameters.Add(entityFields);
         parameterTypes.Add(typeof(ITypeDescriptorCollection));

         // Execute the method using reflection.
         object[] parametersArray = parameters.ToArray();
         MethodInfo proxyMethodInfo = typeof(GlymaRepositoryProxy).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance, null, parameterTypes.ToArray(), null);
         if (proxyMethodInfo == null)
         {
            throw new Microsoft.BusinessData.Runtime.RuntimeException("A method with the name \"" + methodName + "\" with the specified parameters could not be found in the proxy.");
         }
         object result = proxyMethodInfo.Invoke(proxy, parametersArray);

         // Set the parameter values using the results of the method execution.
         foreach (IParameter parameterDefinition in parameterDefinitions)
         {
            if (parameterDefinition.Direction == DirectionType.Return)
            {
               methodArgs[parameterDefinition.OrdinalNumber] = result;
            }
            else
            {
               methodArgs[parameterDefinition.OrdinalNumber] = parametersArray[parameterDefinition.OrdinalNumber];
            }
         }
      }
   }
}
