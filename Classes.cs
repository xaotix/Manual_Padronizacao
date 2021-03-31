using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Manual_Padronizacao
{

    public class ArvoreXML
    {
        public static string CabValor = "value";
        public static string CabIcone = "icone";
        public static string CabTag = "tag";
        public static string Nome = "ArvoreXML";
        XmlDocument DocumentoXML;


        public ArvoreXML()
        {
        }

        public void ArvoreParaXML(TreeView treeView, String path)
        {
            DocumentoXML = new XmlDocument();
            DocumentoXML.AppendChild(DocumentoXML.CreateElement("ROOT"));
            ExportarRecursivo(DocumentoXML.DocumentElement, treeView.Nodes);
            DocumentoXML.Save(path);
        }

        public void XMLParaArvore(String Caminho, TreeView Arvore)
        {
            if (File.Exists(Caminho))
            {
                DocumentoXML = new XmlDocument();

                DocumentoXML.Load(Caminho);
                Arvore.Nodes.Clear();
                ImportarRecursivo(Arvore.Nodes, DocumentoXML.DocumentElement.ChildNodes);
            }
            else
            {
                MessageBox.Show("Arquivo " + Caminho + " não encontrado.");
            }
            
        }

        private XmlNode ExportarRecursivo(XmlNode No, TreeNodeCollection ColecaoNos)
        {
            XmlNode NoXML = null;
            foreach (TreeNode treeNode in ColecaoNos)
            {
                NoXML = DocumentoXML.CreateElement(Nome);

                NoXML.Attributes.Append(DocumentoXML.CreateAttribute(CabValor));
                NoXML.Attributes.Append(DocumentoXML.CreateAttribute(CabTag));
                NoXML.Attributes.Append(DocumentoXML.CreateAttribute(CabIcone));
                NoXML.Attributes[CabValor].Value = treeNode.Text;
                NoXML.Attributes[CabTag].Value = treeNode.Tag.ToString();
                NoXML.Attributes[CabIcone].Value = treeNode.SelectedImageKey.ToString();


                if (No != null)
                    No.AppendChild(NoXML);

                if (treeNode.Nodes.Count > 0)
                {
                    ExportarRecursivo(NoXML, treeNode.Nodes);
                }
            }
            return NoXML;
        }

        private void ImportarRecursivo(TreeNodeCollection ColecaoNos, XmlNodeList ListadeNos)
        {
            TreeNode treeNode;
            foreach (XmlNode XmlNo in ListadeNos)
            {
                treeNode = new TreeNode("");
                treeNode = new TreeNode(XmlNo.Attributes[CabValor].Value);
                treeNode.ToolTipText = XmlNo.Attributes[CabValor].Value;
                treeNode.Tag = XmlNo.Attributes[CabTag].Value;
                //treeNode.ImageIndex = Convert.ToInt32(XmlNo.Attributes[CabIcone].Value);
                //treeNode.SelectedImageIndex = Convert.ToInt32(XmlNo.Attributes[CabIcone].Value);
                treeNode.ImageKey = XmlNo.Attributes[CabIcone].Value;
                treeNode.SelectedImageKey = XmlNo.Attributes[CabIcone].Value;
                if (XmlNo.ChildNodes.Count > 0)
                {
                    ImportarRecursivo(treeNode.Nodes, XmlNo.ChildNodes);
                }
                ColecaoNos.Add(treeNode);
            }
        }
    }
}