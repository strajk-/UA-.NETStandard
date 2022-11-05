/* ========================================================================
 * Copyright (c) 2005-2019 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Server;
using System.Reflection;
using System.Linq;

namespace StructuresWithArrays
{
    /// <summary>
    /// The node manager factory for test data.
    /// </summary>
    public class StructuresWithArraysManagerFactory : INodeManagerFactory
    {
        /// <inheritdoc/>
        public INodeManager Create(IServerInternal server, ApplicationConfiguration configuration)
        {
            return new StructuresWithArraysNodeManager(server, configuration, NamespacesUris.ToArray());
        }

        /// <inheritdoc/>
        public StringCollection NamespacesUris
        {
            get
            {
                var nameSpaces = new StringCollection {
                    Namespaces.StructuresWithArrays
                };
                return nameSpaces;
            }
        }
    }

    /// <summary>
    /// A node manager for a variety of test data.
    /// </summary>
    public class StructuresWithArraysNodeManager : CustomNodeManager2
    {
        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public StructuresWithArraysNodeManager(Opc.Ua.Server.IServerInternal server, ApplicationConfiguration configuration, string[] namespaceUris)
        :
            base(server, configuration)
        {
            // update the namespaces.
            NamespaceUris = namespaceUris;

            Server.Factory.AddEncodeableTypes(typeof(StructuresWithArraysNodeManager).Assembly.GetExportedTypes().Where(t => t.FullName.StartsWith(typeof(StructuresWithArraysNodeManager).Namespace)));

            // get the configuration for the node manager.
            m_configuration = configuration.ParseExtension<StructuresWithArraysConfiguration>();

            // use suitable defaults if no configuration exists.
            if (m_configuration == null)
            {
                m_configuration = new StructuresWithArraysConfiguration();
            }

            // create the object used to access the test system.
            m_system = new StructuresWithArraysSystem(this, server.NamespaceUris, server.ServerUris);

            // update the default context.
            SystemContext.SystemHandle = m_system;
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="node">The node.</param>
        /// <returns>The new NodeId.</returns>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            uint id = Utils.IncrementIdentifier(ref m_lastUsedId);
            return new NodeId(id, m_namespaceIndex);
        }
        #endregion

        #region INodeManager Members
        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                // ensure the namespace used by the node manager is in the server's namespace table.
                m_namespaceIndex = Server.NamespaceUris.GetIndexOrAppend(Namespaces.StructuresWithArrays);

                base.CreateAddressSpace(externalReferences);

                CreateStructures(SystemContext);
            }
        }

        /// <summary>
        /// Loads a node set from a file or resource and addes them to the set of predefined nodes.
        /// </summary>
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            NodeStateCollection predefinedNodes = new NodeStateCollection();
            predefinedNodes.LoadFromBinaryResource(context, "Quickstarts.Servers.StructuresWithArrays.StructuresWithArrays.PredefinedNodes.uanodes",
                this.GetType().GetTypeInfo().Assembly, true);
            return predefinedNodes;
        }

        /// <summary>
        /// Replaces the generic node with a node specific to the model.
        /// </summary>
        protected override NodeState AddBehaviourToPredefinedNode(ISystemContext context, NodeState predefinedNode)
        {
            var variableNode = predefinedNode as BaseVariableState;
            if (variableNode != null)
            {
                if (variableNode.Value == null)
                {
                    var dataTypeId = variableNode.DataType;
                    if (IsNodeIdInNamespace(variableNode.DataType))
                    {
                        if (variableNode.Parent is BaseVariableTypeState)
                        {
                            return predefinedNode;
                        }

                        switch ((uint)dataTypeId.Identifier)
                        {
                            case DataTypes.Structure_A:
                            {
                                if (variableNode is Structure_A_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_A_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }

                                return activeNode;
                            }
                            case DataTypes.Structure_B:
                            {
                                if (variableNode is Structure_B_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_B_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_C:
                            {
                                if (variableNode is Structure_C_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_C_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_D:
                            {
                                if (variableNode is Structure_D_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_D_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_E:
                            {
                                if (variableNode is Structure_E_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_E_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_F:
                            {
                                if (variableNode is Structure_F_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_F_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_G:
                            {
                                if (variableNode is Structure_G_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_G_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_H:
                            {
                                if (variableNode is Structure_H_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_H_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_J:
                            {
                                if (variableNode is Structure_J_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_J_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_K:
                            {
                                if (variableNode is Structure_K_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_K_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_L:
                            {
                                if (variableNode is Structure_L_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_L_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_M:
                            {
                                if (variableNode is Structure_M_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_M_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_N:
                            {
                                if (variableNode is Structure_N_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_N_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_O:
                            {
                                if (variableNode is Structure_O_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_O_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_P:
                            {
                                if (variableNode is Structure_P_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_P_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_Q:
                            {
                                if (variableNode is Structure_Q_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_Q_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_R:
                            {
                                if (variableNode is Structure_R__TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_R__TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_S:
                            {
                                if (variableNode is Structure_S_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_S_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_T:
                            {
                                if (variableNode is Structure_T_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_T_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.LargeComplexStructure:
                            {
                                if (variableNode is LargeComplexStructureTypeState)
                                {
                                    break;
                                }

                                var activeNode = new LargeComplexStructureTypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }

                            default: break;
                        }
                    }
                }
            }

            var passiveNode = predefinedNode as BaseObjectState;

            if (passiveNode == null)
            {
                return predefinedNode;
            }

            NodeId typeId = passiveNode.TypeDefinitionId;

            if (!IsNodeIdInNamespace(typeId) || typeId.IdType != IdType.Numeric)
            {
                return predefinedNode;
            }

            return predefinedNode;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateStructures(ServerSystemContext systemContext)
        {
            const uint Structures_LargeArray = 6156;
            var structures_largearray = new ExpandedNodeId(Structures_LargeArray, Namespaces.StructuresWithArrays);

            LargeComplexStructureTypeState variable = FindPredefinedNode(
                ExpandedNodeId.ToNodeId(structures_largearray, Server.NamespaceUris),
                typeof(LargeComplexStructureTypeState)) as LargeComplexStructureTypeState;
            if (variable != null)
            {
                m_largeArray = new LargeComplexStructureTypeValue(variable, null, m_lock);
            }

            const uint Structures_Structure_A = 6204;
            var structures_structure_A = new ExpandedNodeId(Structures_Structure_A, Namespaces.StructuresWithArrays);

            Structure_A_TypeState variable2 = FindPredefinedNode(
                ExpandedNodeId.ToNodeId(structures_structure_A, Server.NamespaceUris),
                typeof(Structure_A_TypeState)) as Structure_A_TypeState;
            if (variable2 != null)
            {
                m_structureA = new Structure_A_TypeValue(variable2, null, m_lock);
            }
        }
        #endregion

        #region Private Fields
        private object m_lock = new object();
        private StructuresWithArraysConfiguration m_configuration;
        private ushort m_namespaceIndex;
        private StructuresWithArraysSystem m_system;
        private long m_lastUsedId;
        // Variables
        private LargeComplexStructureTypeValue m_largeArray;
        private Structure_A_TypeValue m_structureA;
        //private Structure_A_TypeValue m_largeArray;
        //private Structure_A_TypeValue m_largeArray;
        #endregion
    }
}
