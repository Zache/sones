﻿/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ChangesetObjects;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InsertPayload;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.PayloadObjects;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceConverter
{
    public static class ServiceRequestFactory
    {
        public static RequestAlterVertexType MakeRequestAlterVertexType(ServiceVertexType myVertexType, ServiceAlterVertexChangeset myChangeset)
        {
            var Request = new RequestAlterVertexType(myVertexType.Name);

            #region Add Attributes
            
            foreach (var toAdd in myChangeset.ToBeAddedProperties)
            {
                Request.AddProperty(toAdd.ToPropertyPredefinition());
            }

            foreach (var toAdd in myChangeset.ToBeAddedIncomingEdges)
            {
                Request.AddIncomingEdge(toAdd.ToIncomingEdgePredefinition());
            }

            foreach (var toAdd in myChangeset.ToBeAddedOutgoingEdges)
            {
                Request.AddOutgoingEdge(toAdd.ToOutgoingEdgePredefinition());
            }

            foreach (var toAdd in myChangeset.ToBeAddedUniques)
            {
                Request.AddUnique(toAdd.ToUniquePredefinition());
            }

            foreach (var toAdd in myChangeset.ToBeAddedMandatories)
            {
                Request.AddMandatory(toAdd.ToMandatoryPredefinition());
            }

            foreach (var toAdd in myChangeset.ToBeAddedIndices)
            {
                Request.AddIndex(toAdd.ToIndexPredefinition());
            }


            #endregion

            #region Remove Attributes

            foreach (var toDel in myChangeset.ToBeRemovedProperties)
            {
                Request.RemoveProperty(toDel.AttributeName);
            }

            foreach (var toDel in myChangeset.ToBeRemovedIncomingEdges)
            {
                Request.RemoveIncomingEdge(toDel.AttributeName);
            }

            foreach (var toDel in myChangeset.ToBeRemovedOutgoingEdges)
            {
                Request.RemoveOutgoingEdge(toDel.AttributeName);
            }

            foreach (var toDel in myChangeset.ToBeRemovedUniques)
            {
                foreach(var unique in toDel.Properties)
                    Request.RemoveUnique(unique);
            }

            foreach (var toDel in myChangeset.ToBeRemovedMandatories)
            {
                Request.RemoveMandatory(toDel.PropertyName);
            }

            foreach (var toDel in myChangeset.ToBeRemovedIndices)
            {
                Request.RemoveIndex(toDel.Name, toDel.Edition);
            }

            #endregion

            #region Rename Task

            myChangeset.ToBeRenamedProperties.Select((tuple, key) => Request.RenameAttribute(tuple.Key, tuple.Value));
            
            #endregion


            if (myChangeset.Comment != null)
                Request.SetComment(myChangeset.Comment);

            if (myChangeset.NewTypeName != null)
                Request.RenameType(myChangeset.Comment);
            
            //todo add unknown attribute

            return Request;
        }

        public static RequestAlterEdgeType MakeRequestAlterEdgeType(ServiceEdgeType myEdgeType, ServiceAlterEdgeChangeset myChangeset)
        {
            var Request = new RequestAlterEdgeType(myEdgeType.Name);

            #region Add Attributes

            foreach (var toAdd in myChangeset.ToBeAddedProperties)
            {
                Request.AddProperty(toAdd.ToPropertyPredefinition());
            }
            
            
            #endregion

            #region Remove Attributes

            foreach (var toDel in myChangeset.ToBeRemovedProperties)
            {
                Request.RemoveProperty(toDel.AttributeName);
            }

            #endregion

            #region Rename Task

            myChangeset.ToBeRenamedProperties.Select((tuple, key) => Request.RenameAttribute(tuple.Key, tuple.Value));

            #endregion

            
            if (myChangeset.Comment != null)
                Request.SetComment(myChangeset.Comment);

            if (myChangeset.NewTypeName != null)
                Request.RenameType(myChangeset.Comment);
                        
            //todo add unknown attribute

            return Request;
        }

        public static RequestCreateVertexType MakeRequestCreateVertexType(ServiceVertexTypePredefinition myVertexTypePredefinition)
        {
            return new RequestCreateVertexType(myVertexTypePredefinition.ToVertexTypePredefinition());
        }

        public static RequestCreateVertexTypes MakeRequestCreateVertexTypes(IEnumerable<ServiceVertexTypePredefinition> myVertexTypePredefinition)
        {
            return new RequestCreateVertexTypes(myVertexTypePredefinition.Select(x => x.ToVertexTypePredefinition()));
        }

        public static RequestCreateEdgeType MakeRequestCreateEdgeType(ServiceEdgeTypePredefinition myEdgeTypePredefinition)
        {
            return new RequestCreateEdgeType(myEdgeTypePredefinition.ToEdgeTypePredefinition());
        }

        public static RequestCreateEdgeTypes MakeRequestCreateEdgeTypes(IEnumerable<ServiceEdgeTypePredefinition> myEdgeTypePredefinition)
        {
            return new RequestCreateEdgeTypes(myEdgeTypePredefinition.Select(x => x.ToEdgeTypePredefinition()));
        }
        
        public static RequestCreateIndex MakeRequestCreateIndex(ServiceIndexPredefinition myIndexPreDef)
        {
            return new RequestCreateIndex(myIndexPreDef.ToIndexPredefinition());
        }

        public static RequestDelete MakeRequestDelete(ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs = null, ServiceDeletePayload myDeletePayload = null)
        {
            RequestGetVertices PreRequest = null;
            if (myVertexIDs != null)
            {
                PreRequest = new RequestGetVertices(myVertexType.Name, myVertexIDs);
            }
            else
            {
                PreRequest = new RequestGetVertices(myVertexType.Name);
            }

            RequestDelete Request = new RequestDelete(PreRequest);
            if (myDeletePayload != null)
            {
                foreach (var toDel in myDeletePayload.ToBeDeletedAttributes)
                {
                    Request.AddAttribute(toDel);

                }
            }
            return Request;
        }

        public static RequestDescribeIndex MakeRequestDescribeIndex(ServiceVertexType myVertexType, String myIndexName)
        {
            if (String.IsNullOrEmpty(myVertexType.Name))
                return new RequestDescribeIndex(myVertexType.Name, myIndexName);
            return null;
        }

        public static RequestDropEdgeType MakeRequestDropEdgeType(ServiceEdgeType myEdgeType)
        {
            RequestDropEdgeType Request = new RequestDropEdgeType(myEdgeType.Name);
            return Request;
        }

        public static RequestDropIndex MakeRequestDropIndex(ServiceVertexType myVertexType, String myIndexName,String myEdition)
        {
            RequestDropIndex Request = new RequestDropIndex(myVertexType.Name, myIndexName, myEdition);
            return Request;
        }

        public static RequestDropVertexType MakeRequestDropVertexType(ServiceVertexType myVertexType)
        {
            RequestDropVertexType Request = new RequestDropVertexType(myVertexType.Name);
            return Request;
        }

        public static RequestGetAllEdgeTypes MakeRequestGetAllEdgeTypes(String myEdition = null)
        {
            RequestGetAllEdgeTypes Request = new RequestGetAllEdgeTypes(myEdition);
            return Request;
        }

        public static RequestGetAllVertexTypes MakeRequestGetAllVertexTypes(String myEdition = null)
        {
            RequestGetAllVertexTypes Request = new RequestGetAllVertexTypes(myEdition);
            return Request;
        }

        public static RequestGetEdgeType MakeRequestGetEdgeType(Int64 myEdgeTypeID, String myEdition = null)
        {
            RequestGetEdgeType Request = new RequestGetEdgeType(myEdgeTypeID,myEdition);
            return Request;
        }

        public static RequestGetEdgeType MakeRequestGetEdgeType(String myEdgeTypeName, String myEdition = null)
        {
            RequestGetEdgeType Request = new RequestGetEdgeType(myEdgeTypeName, myEdition);
            return Request;
        }

        public static RequestGetVertex MakeRequestGetVertex(ServiceVertexType myVertexType, Int64 myVertexID, String myEdition = null)
        {
            return new RequestGetVertex(myVertexType.Name, myVertexID, myEdition);
        }

        public static RequestGetVertex MakeRequestGetVertex(Int64 myVertexTypeID, Int64 myVertexID, String myEdition = null)
        {
            return new RequestGetVertex(myVertexTypeID, myVertexID, myEdition);
        }

        public static RequestGetVertexCount MakeRequestGetVertexCount(ServiceVertexType myVertexTypeName)
        {
            return new RequestGetVertexCount(myVertexTypeName.Name);
        }
        
        public static RequestGetVertexType MakeRequestGetVertexType(String myVertexTypeName)
        {
            RequestGetVertexType Request = new RequestGetVertexType(myVertexTypeName);
            return Request;
        }

        public static RequestGetVertexType MakeRequestGetVertexType(Int64 myVertexTypeID)
        {
            RequestGetVertexType Request = new RequestGetVertexType(myVertexTypeID);
            return Request;
        }

        public static RequestGetVertices MakeRequestGetVertices(ServiceVertexType myVertexType)
        {
            return new RequestGetVertices(myVertexType.Name);
        }

        public static RequestUpdate MakeRequestUpdate(ServiceVertexType myVertexType, IEnumerable<Int64> myVertexIDs, ServiceUpdateChangeset myUpdateChangeset)
        {
            #region PreRequest

            
            RequestGetVertices PreRequest = null;
            if (myVertexIDs != null)
            {
                PreRequest = new RequestGetVertices(myVertexType.Name, myVertexIDs);
            }
            else
            {
                PreRequest = new RequestGetVertices(myVertexType.Name);
            }

            RequestUpdate Request = new RequestUpdate(PreRequest);

            if (!String.IsNullOrEmpty(myUpdateChangeset.Comment))
                Request.UpdateComment(myUpdateChangeset.Comment);

            if (!String.IsNullOrEmpty(myUpdateChangeset.Edition))
                Request.UpdateEdition(myUpdateChangeset.Edition);

            #endregion

            #region Update Edges

            foreach (var EdgeType in myUpdateChangeset.ToBeUpdatedEdges)
            {
                if(EdgeType is ServiceSingleEdgeInstance)
                {
                    var SingleEdge = EdgeType as ServiceSingleEdgeInstance;
                    
                    EdgePredefinition def = new EdgePredefinition();
                    def.Comment = SingleEdge.Comment;
                    def.AddVertexID(SingleEdge.TargetVertex.TypeID,SingleEdge.TargetVertex.VertexID);
                    
                    Request.UpdateEdge(def);

                }
                else if(EdgeType is ServiceHyperEdgeInstance)
                {
                    var HyperEdge = EdgeType as ServiceHyperEdgeInstance;

                    EdgePredefinition def = new EdgePredefinition();
                    def.Comment = HyperEdge.Comment;
                    
                    foreach(var SingleEdge in HyperEdge.SingleEdges)
                    {
                        EdgePredefinition InnerEdge = new EdgePredefinition();
                        InnerEdge.Comment = SingleEdge.Comment;
                        InnerEdge.AddVertexID(SingleEdge.TargetVertex.TypeID,SingleEdge.TargetVertex.VertexID);
                    
                        def.AddEdge(InnerEdge);
                    
                    }
                    
                    Request.UpdateEdge(def);
                    
                }
                
            }
            
            #endregion          

            #region Properties
            
            foreach (var toAdd in myUpdateChangeset.ToBeUpdatedUnstructuredProperties)
            {
                Request.UpdateUnstructuredProperty(toAdd.Key,toAdd.Value);
            }

            foreach (var toAdd in myUpdateChangeset.ToBeUpdatedStructuredProperties)
            {
                Request.UpdateStructuredProperty(toAdd.Key, toAdd.Value);
            }

            #endregion
                
            return Request;
        }

        public static RequestInsertVertex MakeRequestInsertVertex(String myVertexTypeName, ServiceInsertPayload myPayload)
        {
            RequestInsertVertex Request = new RequestInsertVertex(myVertexTypeName);

            if(!String.IsNullOrEmpty(myPayload.Comment))
                Request.SetComment(myPayload.Comment);

            if(!String.IsNullOrEmpty(myPayload.Edition))
                Request.SetEdition(myPayload.Edition);

            if (myPayload.UUID != 0)
                Request.SetUUID(myPayload.UUID);

            if (myPayload.StructuredProperties != null)
            {
                foreach (var toInsert in myPayload.StructuredProperties)
                    Request.AddStructuredProperty(toInsert.PropertyName, toInsert.PropertyValue as IComparable);
            }

            if (myPayload.UnstructuredProperties != null)
            {
                foreach (var toInsert in myPayload.UnstructuredProperties)
                    Request.AddUnstructuredProperty(toInsert.PropertyName, toInsert.PropertyValue);
            }
            
            //todo implement unknown property
            return Request;
        }

        public static RequestRebuildIndices MakeRequestRebuildIndices(IEnumerable<string> myTypeNames)
        {
            RequestRebuildIndices Request = new RequestRebuildIndices(myTypeNames);
            return Request;
        }

        public static RequestStatistics MakeRequestStatistics(Int64 myMilliseconds)
        {
            RequestStatistics Request = new RequestStatistics(TimeSpan.FromMilliseconds(myMilliseconds));
            return Request;
        }

        public static RequestTraverseVertex MakeRequestTraverseVertex()
        {
            return null; //This Request is currently unused, because of the fact there is no way to deliver the logic for graph traversals;
        }

        public static RequestClear MakeRequestClear()
        {
           return new RequestClear();
        }
        
    }
}