using Assimp;

using System.Collections.Generic;
using System.IO;

using MeshData = Assimp.Mesh;
using D3DDevice = SharpDX.Direct3D11.Device;
using SharpDX;

namespace D3D11Introduction.utils
{
    public static class MeshManager
    {
        private static Dictionary<string, RenderMesh> mMeshList = new Dictionary<string, RenderMesh>();
        private static Dictionary<string, Texture> mTextureList = new Dictionary<string, Texture>();
        private static D3DDevice mDevice;
        private static AssimpContext mAssetContent = new AssimpContext();
        private static string mRootPath = string.Empty;

        public static void Initialize(D3DDevice device)
        {
            mDevice = device;
            mRootPath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
        }

        public static RenderMesh LoadMesh(string filename)
        {
            if (mMeshList.ContainsKey(filename))
            {
                return mMeshList[filename];
            }

            Scene scene = mAssetContent.ImportFile(GetPathToFile(filename));

            RenderMesh result = new RenderMesh();
            Texture diffuseTexture = null;
            if (scene.HasMaterials)
            {
                foreach (var material in scene.Materials)
                {
                    foreach (var diffueseMaterial in material.GetMaterialTextures(TextureType.Diffuse))
                    {
                        string textureFilePath = GetPathToFile(diffueseMaterial.FilePath);
                        if (!mTextureList.ContainsKey(textureFilePath))
                        {
                            diffuseTexture = new Texture();
                            diffuseTexture.Initialize(mDevice, GetPathToFile(diffueseMaterial.FilePath));

                            mTextureList.Add(GetPathToFile(diffueseMaterial.FilePath), diffuseTexture);
                        }
                    }
                }
            }

            if (scene.HasMeshes)
            {
                foreach (MeshData mesh in scene.Meshes)
                {
                    int primitiveCount = mesh.FaceCount * 3;
                    utils.RenderMesh.MeshFormat[] renderMeshData = new utils.RenderMesh.MeshFormat[primitiveCount]; 
                    int index = 0;
                    foreach (var face in mesh.Faces)
                    {
                        // ensure the data is triangulated.
                        if (face.IndexCount != 3)
                        {
                            System.Console.WriteLine("Unable to process data with faces having > 3 vertices");
                            continue;
                        }

                        // Grab verts, colors, normals and UVs
                        // For now, assume that the color channel isn't populated
                        renderMeshData[index].position.X = mesh.Vertices[face.Indices[0]].X;
                        renderMeshData[index].position.Y = mesh.Vertices[face.Indices[0]].Y;
                        renderMeshData[index].position.Z = mesh.Vertices[face.Indices[0]].Z;
                        renderMeshData[index].position.W = 1.0f;
                        renderMeshData[index].normal.X = mesh.Normals[face.Indices[0]].X;
                        renderMeshData[index].normal.Y = mesh.Normals[face.Indices[0]].Y;
                        renderMeshData[index].normal.Z = mesh.Normals[face.Indices[0]].Z;
                        renderMeshData[index].normal.W = 1.0f;
                        renderMeshData[index].color = new SharpDX.Vector4(1.0f);  // Not supporting Vertex Color channels
                        renderMeshData[index].UV.X = mesh.TextureCoordinateChannels[0][face.Indices[0]].X;
                        renderMeshData[index].UV.Y = mesh.TextureCoordinateChannels[0][face.Indices[0]].Y;
                        index++;

                        renderMeshData[index].position.X = mesh.Vertices[face.Indices[1]].X;
                        renderMeshData[index].position.Y = mesh.Vertices[face.Indices[1]].Y;
                        renderMeshData[index].position.Z = mesh.Vertices[face.Indices[1]].Z;
                        renderMeshData[index].position.W = 1.0f;
                        renderMeshData[index].normal.X = mesh.Normals[face.Indices[1]].X;
                        renderMeshData[index].normal.Y = mesh.Normals[face.Indices[1]].Y;
                        renderMeshData[index].normal.Z = mesh.Normals[face.Indices[1]].Z;
                        renderMeshData[index].normal.W = 1.0f;
                        renderMeshData[index].color = new SharpDX.Vector4(1.0f);
                        renderMeshData[index].UV.X = mesh.TextureCoordinateChannels[0][face.Indices[0]].X;
                        renderMeshData[index].UV.Y = mesh.TextureCoordinateChannels[0][face.Indices[0]].Y;
                        index++;

                        renderMeshData[index].position.X = mesh.Vertices[face.Indices[2]].X;
                        renderMeshData[index].position.Y = mesh.Vertices[face.Indices[2]].Y;
                        renderMeshData[index].position.Z = mesh.Vertices[face.Indices[2]].Z;
                        renderMeshData[index].position.W = 1.0f;
                        renderMeshData[index].normal.X = mesh.Normals[face.Indices[2]].X;
                        renderMeshData[index].normal.Y = mesh.Normals[face.Indices[2]].Y;
                        renderMeshData[index].normal.Z = mesh.Normals[face.Indices[2]].Z;
                        renderMeshData[index].normal.W = 1.0f;
                        renderMeshData[index].color = new SharpDX.Vector4(1.0f);
                        renderMeshData[index].UV.X = mesh.TextureCoordinateChannels[0][face.Indices[0]].X;
                        renderMeshData[index].UV.Y = mesh.TextureCoordinateChannels[0][face.Indices[0]].Y;
                        index++;
                    }
                    result.AddMesh(mDevice, renderMeshData, primitiveCount);
                }
            }

            if (scene.HasMaterials)
            {
                RenderMaterial material = new RenderMaterial();
                Material sceneMaterial = scene.Materials[0];
                material.Ambient = new Vector4(sceneMaterial.ColorAmbient.R, sceneMaterial.ColorAmbient.G, sceneMaterial.ColorAmbient.B, sceneMaterial.ColorAmbient.A);
                material.Diffuse = new Vector4(sceneMaterial.ColorDiffuse.R, sceneMaterial.ColorDiffuse.G, sceneMaterial.ColorDiffuse.B, sceneMaterial.ColorDiffuse.A);
                material.DiffuseMap = diffuseTexture;

                result.Material = material;
            }

            return result;
        }

        private static string GetPathToFile(string filename)
        {
            string result = Path.Combine(mRootPath, filename);

            if (!File.Exists(result))
            {
                string undotted = filename.Replace("..\\", "");
                undotted = undotted.Replace("../", "");
                result = Path.Combine(mRootPath, undotted);
                if (!File.Exists(result))
                {
                    result = string.Empty;
                }
            }
            return result;
        }
    }
}
