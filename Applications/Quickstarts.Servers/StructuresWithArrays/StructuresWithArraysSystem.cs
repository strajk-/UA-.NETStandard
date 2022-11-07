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

using System.CodeDom.Compiler;
using System.Linq.Expressions;
using Opc.Ua;
using Opc.Ua.Server;

namespace StructuresWithArrays
{
    public class StructuresWithArraysSystem
    {
        public StructuresWithArraysSystem(INodeManager2 system, NamespaceTable namespaceUris, StringTable serverUris)
        {
            m_generator = new Opc.Ua.Test.DataGenerator(null);
            m_generator.NamespaceUris = namespaceUris;
            m_generator.ServerUris = serverUris;
            m_system = system;
        }

        /// <summary>
        /// The data generator for the system.
        /// </summary>
        public Opc.Ua.Test.DataGenerator DataGenerator => m_generator;

        /// <summary>
        /// 
        /// </summary>
        public Structure_A GetRandomStructureA()
        {
            return new Structure_A() {
                Double_Field = m_generator.GetRandomDouble(),
                Duration_Field = m_generator.GetRandomDouble()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public Structure_B GetRandomStructureB()
        {
            return new Structure_B() {
                Duration_Field = m_generator.GetRandomDouble(),
                Int32_Field = m_generator.GetRandomInt32()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public LargeComplexStructure GetRandomLargeComplexStructure()
        {
            return new LargeComplexStructure {
                Scalar_Structure_A = GetRandomStructureA(),
                Scalar_Structure_B = GetRandomStructureB(),
                Scalar_Boolean = m_generator.GetRandomBoolean(),
                Scalar_Duration = m_generator.GetRandomDouble(),
                Scalar_String_within_max_length = m_generator.GetRandomString(),
            };
        }


        #region Private Fields
        private object m_lock = new object();
        private Opc.Ua.Test.DataGenerator m_generator;
        private INodeManager2 m_system;
        #endregion
    }
}
