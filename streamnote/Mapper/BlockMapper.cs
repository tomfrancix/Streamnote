using System.Collections.Generic;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Mapper
{
    /// <summary>
    /// Mapper for blocks.
    /// </summary>
    public class BlockMapper
    {
        /// <summary>
        /// Map a list of blocks.
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<PostBlockDescriptor> MapDescriptors(List<PostBlock> blocks)
        {
            var blockDescriptors = new List<PostBlockDescriptor>();

            if (blocks != null)
            {
                foreach (var block in blocks)
                {
                    blockDescriptors.Add(MapDescriptor(block));
                }
            }

            return blockDescriptors;
        }

        /// <summary>
        /// Map a single block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PostBlockDescriptor MapDescriptor(PostBlock block)
        {
            return new PostBlockDescriptor
            {
                Id = block.Id,
                Type = block.Type,
                Text = block.Text,
                FullS3Location = block.FullS3Location,
                Bytes = block.Bytes,
                ImageContentType = block.ImageContentType,
                OutputContainerIdentifier = "outputContainerIdentifier" + block.Id,
                OutputTextIdentifier = "outputTextIdentifier" + block.Id,
                EditorIdentifier = "editorIdentifier" + block.Id,
                MainEditorIdentifier = "mainEditorIdentifier" + block.Id,
                EditorOutputIdentifier = "editorOutputIdentifier" + block.Id,
                OpenBlockEditorButtonIdentifier = "openBlockEditorButtonIdentifier" + block.Id,
            };
        }
    }
}
