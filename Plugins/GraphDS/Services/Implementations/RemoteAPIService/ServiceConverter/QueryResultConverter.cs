﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.GraphQL.Result;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceConverter
{
    // This Converter is currently unused, because there is no plan to implement the 'Query()' method in the API
    // VertexView / EdgeView - Objects are in general for data evaluating purposes and not for the interaction with the GraphDB - the InstanceObjects 
    // of the RemoteAPI Service are not suitable and equivalent.

    public class QueryResultConverter
    {
        private IGraphDS _GraphDS;

        public QueryResultConverter(IGraphDS myGraphDS)
        {
            _GraphDS = myGraphDS;
        }
        
        
        public  List<Dictionary<ServiceVertexInstance, List<ServiceEdgeInstance>>> ConvertVertexViewList(IEnumerable<IVertexView> myVertices)
        {
            List<Dictionary<ServiceVertexInstance, List<ServiceEdgeInstance>>> ReturnView = new List<Dictionary<ServiceVertexInstance, List<ServiceEdgeInstance>>>();

            foreach (var VertexView in myVertices)
            {
                //todo implement convert method
            }


            return ReturnView;
        }

        private  Dictionary<ServiceVertexInstance, List<ServiceEdgeInstance>> ConvertVertexView(IVertexView myVertex)
        {
            Dictionary<ServiceVertexInstance, List<ServiceEdgeInstance>> ReturnVertexView = new Dictionary<ServiceVertexInstance, List<ServiceEdgeInstance>>();


            

            return ReturnVertexView;
        }

        private  List<ServiceEdgeInstance> ConvertEdgeViewList(IEnumerable<IEdgeView> myEdges)
        {
            List<ServiceEdgeInstance> ReturnEdges = new List<ServiceEdgeInstance>();

            //todo implement convert method

            return ReturnEdges;
        }

        private  ServiceEdgeInstance ConvertEdgeView(IEdgeView myEdge)
        {
            if (myEdge is ISingleEdgeView)
            {
                ServiceSingleEdgeInstance ReturnEdge = new ServiceSingleEdgeInstance();
                var EdgeInstance =  myEdge as ISingleEdgeView;

                var EdgeType = EdgeInstance.GetProperty<Int64>("EdgeTypeID");
                var EdgeID = EdgeInstance.GetProperty<Int64>("EdgeID");

                


                return ReturnEdge;
            }
            else if (myEdge is IHyperEdgeView)
            {
                ServiceHyperEdgeInstance ReturnEdge = new ServiceHyperEdgeInstance();
                var Edge = myEdge as IHyperEdgeView;

                //todo implement convert method

                return ReturnEdge;
            }



            return null;
        }

    }
}