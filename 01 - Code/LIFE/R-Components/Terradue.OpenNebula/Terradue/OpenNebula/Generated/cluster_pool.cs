// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.17020
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

// 
//This source code was auto-generated by MonoXSD
//
namespace Terradue.OpenNebula {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class CLUSTER_POOL {
        
        private CLUSTER[] cLUSTERField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CLUSTER")]
        public CLUSTER[] CLUSTER {
            get {
                return this.cLUSTERField;
            }
            set {
                this.cLUSTERField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17020")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=true)]
    public partial class CLUSTER {
        
        private string iDField;
        
        private string nAMEField;
        
        private string[] hOSTSField;
        
        private string[] dATASTORESField;
        
        private string[] vNETSField;
        
        private object tEMPLATEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string ID {
            get {
                return this.iDField;
            }
            set {
                this.iDField = value;
            }
        }
        
        /// <remarks/>
        public string NAME {
            get {
                return this.nAMEField;
            }
            set {
                this.nAMEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItem(ElementName="ID", IsNullable=false)]
        public string[] HOSTS {
            get {
                return this.hOSTSField;
            }
            set {
                this.hOSTSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItem(ElementName="ID", IsNullable=false)]
        public string[] DATASTORES {
            get {
                return this.dATASTORESField;
            }
            set {
                this.dATASTORESField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItem(ElementName="ID", IsNullable=false)]
        public string[] VNETS {
            get {
                return this.vNETSField;
            }
            set {
                this.vNETSField = value;
            }
        }
        
        /// <remarks/>
        public object TEMPLATE {
            get {
                return this.tEMPLATEField;
            }
            set {
                this.tEMPLATEField = value;
            }
        }
    }
}
