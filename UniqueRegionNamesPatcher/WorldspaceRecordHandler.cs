using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UniqueRegionNamesPatcher.Extensions;
using UniqueRegionNamesPatcher.Utility;

namespace UniqueRegionNamesPatcher
{
    public class WorldspaceRecordHandler
    {
        public WorldspaceRecordHandler(UrnRegionMap regionMap) => RegionMap = regionMap;
        /// <summary>
        /// The <see cref="UrnRegionMap"/> associated with this record handler instance.
        /// </summary>
        public UrnRegionMap RegionMap { get; }
        private static readonly int _logTypeMarginMinSize = nameof(WorldspaceSubBlock).Length + 2;
        private static readonly int _logMarginMinSize = 36;

        public bool AppliesTo(IWorldspaceGetter wrld) => RegionMap.WorldspaceFormKey.Equals(wrld.FormKey);

        #region LoggingMethods
        private static string? ResolveMethodName(string name)
        {
            if (name.Equals(nameof(ProcessWorldspace)))
                return nameof(Worldspace);
            else if (name.Equals(nameof(ProcessWorldspaceBlock)))
                return nameof(WorldspaceBlock);
            else if (name.Equals(nameof(ProcessWorldspaceSubBlock)))
                return nameof(WorldspaceSubBlock);
            else if (name.Equals(nameof(ProcessExteriorCell)))
                return nameof(Cell);
            return null;
        }
        private static string MakeMargin(int len) => len < 0 ? string.Empty : new(' ', len);
        private static string MakeMessage(string message, string? additional = null, [CallerMemberName] string caller = "")
        {
            string? callerProcessingType = ResolveMethodName(caller);
            string typename = callerProcessingType is null ? string.Empty : $"[{callerProcessingType}]";

            string additionalData = additional is null ? string.Empty : $"({additional})";

            return $"{typename}{MakeMargin(_logTypeMarginMinSize - typename.Length)} {additionalData}{MakeMargin(_logMarginMinSize - additionalData.Length)} {message}";
        }
        #endregion LoggingMethods

        #region ProcessingMethods
        public Worldspace? ProcessWorldspace(IWorldspaceGetter wrldGetter)
        {
            List<WorldspaceBlock> l = new();
            foreach (var item in wrldGetter.SubCells)
            {
                if (ProcessWorldspaceBlock(item) is WorldspaceBlock block)
                {
                    l.Add(block);
                }
            }
            if (l.Count > 0)
            {
                Console.WriteLine(MakeMessage($"Applying {l.Count} {nameof(WorldspaceBlock)} changes.", wrldGetter.EditorID));
                var copy = wrldGetter.DeepCopy();
                copy.SubCells.AddOrReplaceRange(l);
                return copy;
            }
            else return null;
        }
        private WorldspaceBlock? ProcessWorldspaceBlock(IWorldspaceBlockGetter blockGetter)
        {
            List<WorldspaceSubBlock> l = new();
            foreach (var item in blockGetter.Items)
            {
                if (ProcessWorldspaceSubBlock(item) is WorldspaceSubBlock subBlock)
                {
                    l.Add(subBlock);
                }
            }
            if (l.Count > 0)
            {
                Console.WriteLine(MakeMessage($"Applying {l.Count} {nameof(WorldspaceSubBlock)} changes.", $"{blockGetter.BlockNumberX}, {blockGetter.BlockNumberY}"));
                var copy = blockGetter.DeepCopy();
                copy.Items.AddOrReplaceRange(l);
                return copy;
            }
            else return null;
        }
        private WorldspaceSubBlock? ProcessWorldspaceSubBlock(IWorldspaceSubBlockGetter subBlockGetter)
        {
            List<Cell> l = new();
            foreach (var item in subBlockGetter.Items)
            {
                if (ProcessExteriorCell(item) is Cell cell)
                {
                    l.Add(cell);
                }
            }
            if (l.Count > 0)
            {
                Console.WriteLine(MakeMessage($"Applying {l.Count} {nameof(Cell)} changes.", $"{subBlockGetter.BlockNumberX}, {subBlockGetter.BlockNumberY}"));
                var copy = subBlockGetter.DeepCopy();
                copy.Items.AddOrReplaceRange(l);
                return copy;
            }
            else return null;
        }
        private Cell? ProcessExteriorCell(ICellGetter cell)
        {
            if (cell.Grid is not null)
            {
                Point coord = new(cell.Grid.Point.X, cell.Grid.Point.Y);

                var regions = RegionMap.GetFormLinksForPos(coord);

                if (regions.Count > 0)
                {
                    Console.WriteLine(MakeMessage($"Adding {regions.Count} region{(regions.Count.Equals(1) ? "" : "s")} to {cell.EditorID ?? "Wilderness"}", $"{coord.X}, {coord.Y}"));

                    var copy = cell.DeepCopy();

                    if (copy.Regions is null)
                        copy.Regions = new();

                    copy.Regions.AddOrReplaceRange(regions);

                    return copy;
                }
                else
                {
                    Console.WriteLine(MakeMessage($"No regions were found for this cell."));
                }
            }
            return null;
        }
        #endregion ProcessingMethods
    }
}
