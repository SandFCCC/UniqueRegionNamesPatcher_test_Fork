using Mutagen.Bethesda.Skyrim;
using System.Drawing;
using UniqueRegionNamesPatcher.Extensions;
using UniqueRegionNamesPatcher.Utility;

namespace UniqueRegionNamesPatcher
{
    /// <summary>
    /// Processes <see cref="Worldspace"/> records.
    /// </summary>
    public class WorldspaceProcessor
    {
        #region Constructor
        public WorldspaceProcessor(UrnRegionMap regionMap) => this.RegionMap = regionMap;
        #endregion Constructor

        #region Properties
        internal UrnRegionMap RegionMap { get; }
        #endregion Properties

        #region Methods
        public bool AppliesTo(IWorldspaceGetter wrld) => RegionMap.WorldspaceFormKey.Equals(wrld.FormKey);
        public void Process(ref Worldspace worldspace, ref int changes)
        {
            for (int b_i = 0; b_i < worldspace.SubCells.Count; ++b_i)
            {
                WorldspaceBlock block = worldspace.SubCells[b_i];

                for (int s_i = 0; s_i < block.Items.Count; ++s_i)
                {
                    WorldspaceSubBlock subBlock = block.Items[s_i];

                    for (int c_i = 0; c_i < subBlock.Items.Count; ++c_i)
                    {
                        Cell cell = subBlock.Items[c_i];

                        if (cell.Grid is null) continue;

                        Point pos = new(cell.Grid.Point.X, cell.Grid.Point.Y);
                        var regions = RegionMap.GetFormLinksForPos(pos);

                        if (regions.Count == 0) continue;

                        if (cell.Regions is null)
                            cell.Regions = new();

                        cell.Regions.AddOrReplaceRange(regions);

                        subBlock.Items[c_i] = cell;

                        ++changes;
                    } //^ cells
                    block.Items[s_i] = subBlock;
                } //^ subBlocks
                worldspace.SubCells[b_i] = block;
            } //^ blocks
        }
        #endregion Methods
    }
}
