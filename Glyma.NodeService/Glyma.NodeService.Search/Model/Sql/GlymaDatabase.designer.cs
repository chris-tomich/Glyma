﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Glyma.NodeService.Search.Model.Sql
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="MappingToolDatabase")]
	public partial class GlymaDatabaseDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertNode(Node instance);
    partial void UpdateNode(Node instance);
    partial void DeleteNode(Node instance);
    partial void InsertMetadata(Metadata instance);
    partial void UpdateMetadata(Metadata instance);
    partial void DeleteMetadata(Metadata instance);
    #endregion
		
		public GlymaDatabaseDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["MappingToolDatabaseConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public GlymaDatabaseDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GlymaDatabaseDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GlymaDatabaseDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GlymaDatabaseDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Node> Nodes
		{
			get
			{
				return this.GetTable<Node>();
			}
		}
		
		public System.Data.Linq.Table<Metadata> Metadatas
		{
			get
			{
				return this.GetTable<Metadata>();
			}
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.BasicSearch")]
		public ISingleResult<BasicSearchResult> BasicSearch([global::System.Data.Linq.Mapping.ParameterAttribute(Name="DomainId", DbType="UniqueIdentifier")] System.Nullable<System.Guid> domainId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="SearchTerms", DbType="NVarChar(200)")] string searchTerms)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), domainId, searchTerms);
			return ((ISingleResult<BasicSearchResult>)(result.ReturnValue));
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Nodes")]
	public partial class Node : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _NodeUid;
		
		private string _NodeOriginalId;
		
		private System.Nullable<System.Guid> _NodeTypeUid;
		
		private System.Nullable<System.Guid> _DomainUid;
		
		private EntitySet<Metadata> _Metadatas;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnNodeUidChanging(System.Guid value);
    partial void OnNodeUidChanged();
    partial void OnNodeOriginalIdChanging(string value);
    partial void OnNodeOriginalIdChanged();
    partial void OnNodeTypeUidChanging(System.Nullable<System.Guid> value);
    partial void OnNodeTypeUidChanged();
    partial void OnDomainUidChanging(System.Nullable<System.Guid> value);
    partial void OnDomainUidChanged();
    #endregion
		
		public Node()
		{
			this._Metadatas = new EntitySet<Metadata>(new Action<Metadata>(this.attach_Metadatas), new Action<Metadata>(this.detach_Metadatas));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NodeUid", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid NodeUid
		{
			get
			{
				return this._NodeUid;
			}
			set
			{
				if ((this._NodeUid != value))
				{
					this.OnNodeUidChanging(value);
					this.SendPropertyChanging();
					this._NodeUid = value;
					this.SendPropertyChanged("NodeUid");
					this.OnNodeUidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NodeOriginalId", DbType="NVarChar(50)")]
		public string NodeOriginalId
		{
			get
			{
				return this._NodeOriginalId;
			}
			set
			{
				if ((this._NodeOriginalId != value))
				{
					this.OnNodeOriginalIdChanging(value);
					this.SendPropertyChanging();
					this._NodeOriginalId = value;
					this.SendPropertyChanged("NodeOriginalId");
					this.OnNodeOriginalIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NodeTypeUid", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> NodeTypeUid
		{
			get
			{
				return this._NodeTypeUid;
			}
			set
			{
				if ((this._NodeTypeUid != value))
				{
					this.OnNodeTypeUidChanging(value);
					this.SendPropertyChanging();
					this._NodeTypeUid = value;
					this.SendPropertyChanged("NodeTypeUid");
					this.OnNodeTypeUidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DomainUid", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> DomainUid
		{
			get
			{
				return this._DomainUid;
			}
			set
			{
				if ((this._DomainUid != value))
				{
					this.OnDomainUidChanging(value);
					this.SendPropertyChanging();
					this._DomainUid = value;
					this.SendPropertyChanged("DomainUid");
					this.OnDomainUidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Node_Metadata", Storage="_Metadatas", ThisKey="NodeUid", OtherKey="NodeUid")]
		public EntitySet<Metadata> Metadatas
		{
			get
			{
				return this._Metadatas;
			}
			set
			{
				this._Metadatas.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Metadatas(Metadata entity)
		{
			this.SendPropertyChanging();
			entity.Node = this;
		}
		
		private void detach_Metadatas(Metadata entity)
		{
			this.SendPropertyChanging();
			entity.Node = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Metadata")]
	public partial class Metadata : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private System.Guid _MetadataId;
		
		private System.Nullable<System.Guid> _MetadataTypeUid;
		
		private System.Nullable<System.Guid> _NodeUid;
		
		private System.Nullable<System.Guid> _RelationshipUid;
		
		private System.Nullable<System.Guid> _DescriptorTypeUid;
		
		private string _MetadataName;
		
		private string _MetadataValue;
		
		private EntityRef<Node> _Node;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnMetadataIdChanging(System.Guid value);
    partial void OnMetadataIdChanged();
    partial void OnMetadataTypeUidChanging(System.Nullable<System.Guid> value);
    partial void OnMetadataTypeUidChanged();
    partial void OnNodeUidChanging(System.Nullable<System.Guid> value);
    partial void OnNodeUidChanged();
    partial void OnRelationshipUidChanging(System.Nullable<System.Guid> value);
    partial void OnRelationshipUidChanged();
    partial void OnDescriptorTypeUidChanging(System.Nullable<System.Guid> value);
    partial void OnDescriptorTypeUidChanged();
    partial void OnMetadataNameChanging(string value);
    partial void OnMetadataNameChanged();
    partial void OnMetadataValueChanging(string value);
    partial void OnMetadataValueChanged();
    #endregion
		
		public Metadata()
		{
			this._Node = default(EntityRef<Node>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MetadataId", DbType="UniqueIdentifier NOT NULL", IsPrimaryKey=true)]
		public System.Guid MetadataId
		{
			get
			{
				return this._MetadataId;
			}
			set
			{
				if ((this._MetadataId != value))
				{
					this.OnMetadataIdChanging(value);
					this.SendPropertyChanging();
					this._MetadataId = value;
					this.SendPropertyChanged("MetadataId");
					this.OnMetadataIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MetadataTypeUid", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> MetadataTypeUid
		{
			get
			{
				return this._MetadataTypeUid;
			}
			set
			{
				if ((this._MetadataTypeUid != value))
				{
					this.OnMetadataTypeUidChanging(value);
					this.SendPropertyChanging();
					this._MetadataTypeUid = value;
					this.SendPropertyChanged("MetadataTypeUid");
					this.OnMetadataTypeUidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NodeUid", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> NodeUid
		{
			get
			{
				return this._NodeUid;
			}
			set
			{
				if ((this._NodeUid != value))
				{
					if (this._Node.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnNodeUidChanging(value);
					this.SendPropertyChanging();
					this._NodeUid = value;
					this.SendPropertyChanged("NodeUid");
					this.OnNodeUidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RelationshipUid", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> RelationshipUid
		{
			get
			{
				return this._RelationshipUid;
			}
			set
			{
				if ((this._RelationshipUid != value))
				{
					this.OnRelationshipUidChanging(value);
					this.SendPropertyChanging();
					this._RelationshipUid = value;
					this.SendPropertyChanged("RelationshipUid");
					this.OnRelationshipUidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DescriptorTypeUid", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> DescriptorTypeUid
		{
			get
			{
				return this._DescriptorTypeUid;
			}
			set
			{
				if ((this._DescriptorTypeUid != value))
				{
					this.OnDescriptorTypeUidChanging(value);
					this.SendPropertyChanging();
					this._DescriptorTypeUid = value;
					this.SendPropertyChanged("DescriptorTypeUid");
					this.OnDescriptorTypeUidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MetadataName", DbType="NVarChar(50)")]
		public string MetadataName
		{
			get
			{
				return this._MetadataName;
			}
			set
			{
				if ((this._MetadataName != value))
				{
					this.OnMetadataNameChanging(value);
					this.SendPropertyChanging();
					this._MetadataName = value;
					this.SendPropertyChanged("MetadataName");
					this.OnMetadataNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MetadataValue", DbType="NVarChar(MAX)")]
		public string MetadataValue
		{
			get
			{
				return this._MetadataValue;
			}
			set
			{
				if ((this._MetadataValue != value))
				{
					this.OnMetadataValueChanging(value);
					this.SendPropertyChanging();
					this._MetadataValue = value;
					this.SendPropertyChanged("MetadataValue");
					this.OnMetadataValueChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Node_Metadata", Storage="_Node", ThisKey="NodeUid", OtherKey="NodeUid", IsForeignKey=true)]
		public Node Node
		{
			get
			{
				return this._Node.Entity;
			}
			set
			{
				Node previousValue = this._Node.Entity;
				if (((previousValue != value) 
							|| (this._Node.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Node.Entity = null;
						previousValue.Metadatas.Remove(this);
					}
					this._Node.Entity = value;
					if ((value != null))
					{
						value.Metadatas.Add(this);
						this._NodeUid = value.NodeUid;
					}
					else
					{
						this._NodeUid = default(Nullable<System.Guid>);
					}
					this.SendPropertyChanged("Node");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	public partial class BasicSearchResult
	{
		
		private System.Guid _NodeUid;
		
		private string _NodeOriginalId;
		
		private System.Nullable<System.Guid> _NodeTypeUid;
		
		private System.Nullable<System.Guid> _DomainUid;
		
		public BasicSearchResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NodeUid", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid NodeUid
		{
			get
			{
				return this._NodeUid;
			}
			set
			{
				if ((this._NodeUid != value))
				{
					this._NodeUid = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NodeOriginalId", DbType="NVarChar(50)")]
		public string NodeOriginalId
		{
			get
			{
				return this._NodeOriginalId;
			}
			set
			{
				if ((this._NodeOriginalId != value))
				{
					this._NodeOriginalId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NodeTypeUid", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> NodeTypeUid
		{
			get
			{
				return this._NodeTypeUid;
			}
			set
			{
				if ((this._NodeTypeUid != value))
				{
					this._NodeTypeUid = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DomainUid", DbType="UniqueIdentifier")]
		public System.Nullable<System.Guid> DomainUid
		{
			get
			{
				return this._DomainUid;
			}
			set
			{
				if ((this._DomainUid != value))
				{
					this._DomainUid = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
