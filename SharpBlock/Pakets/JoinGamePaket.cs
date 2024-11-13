using SharpBlock.Options;
using SharpNBT;
using SharpNBT.Extensions;

namespace SharpBlock.Pakets
{
    public class JoinGamePaket : OutgoingPaket
    {
        public override int PacketId => 0x26;

        public int EntityId { get; set; }
        public sbyte PreviousGameMode { get; set; } = -1;
        public long HashedSeed { get; set; } = 0;
        public int MaxPlayers { get; set; }
        public WorldsOptions WorldsOptions { get; set; } = null!;

        public override void Write(Stream stream)
        {
            stream.WriteInt(EntityId);
            stream.WriteBoolean(WorldsOptions.IsHardcore);
            stream.WriteUnsignedByte(WorldsOptions.GameMode);
            stream.WriteByte(PreviousGameMode);

            var worldNames = WorldsOptions.Dimensions.Select(x => x.Name).ToList();
            
            // Write World Names
            stream.WriteVarInt(worldNames.Count);
            foreach (var worldName in worldNames)
            {
                stream.WriteString(worldName);
            }

            // Write Dimension Codec
            byte[] dimensionCodecData = BuildDimensionCodec();
            stream.WriteByteArrayWithLength(dimensionCodecData);

            // Write Dimension
            byte[] dimensionData = BuildDimension();
            stream.WriteByteArrayWithLength(dimensionData);

            // Continue with other required fields
            stream.WriteString(WorldsOptions.Name);
            stream.WriteLong(HashedSeed);
            stream.WriteVarInt(MaxPlayers); // Ignored by client
            stream.WriteVarInt(WorldsOptions.ViewDistance);
            stream.WriteVarInt(WorldsOptions.SimulationDistance);
            stream.WriteBoolean(WorldsOptions.ReduceDebugInfo);
            stream.WriteBoolean(WorldsOptions.EnableRespawnScreen);
            stream.WriteBoolean(WorldsOptions.IsDebug);
            stream.WriteBoolean(WorldsOptions.IsFlat);
        }
        
        private byte[] BuildDimensionCodec()
        {
            var dimensionTypesList = new NbtList("value", NbtTagType.Compound);

            foreach (var world in WorldsOptions.Dimensions)
            {
                var dimensionType = new NbtCompound("")
                {
                    new NbtString("name", world.Name),
                    new NbtInt("id", world.Id),
                    new NbtCompound("element")
                    {
                        new NbtByte("piglin_safe", world.PiglinSafe ? (byte)1 : (byte)0),
                        new NbtByte("natural", world.Natural ? (byte)1 : (byte)0),
                        new NbtFloat("ambient_light", world.AmbientLight),
                        new NbtString("infiniburn", world.Infiniburn),
                        new NbtByte("respawn_anchor_works", world.RespawnAnchorWorks ? (byte)1 : (byte)0),
                        new NbtByte("has_skylight", world.HasSkylight ? (byte)1 : (byte)0),
                        new NbtByte("bed_works", world.BedWorks ? (byte)1 : (byte)0),
                        new NbtString("effects", world.Effects),
                        new NbtInt("logical_height", world.LogicalHeight),
                        new NbtDouble("coordinate_scale", world.CoordinateScale),
                        new NbtByte("ultrawarm", world.Ultrawarm ? (byte)1 : (byte)0),
                        new NbtByte("has_ceiling", world.HasCeiling ? (byte)1 : (byte)0),
                        // Add other properties as needed
                    }
                };

                dimensionTypesList.Add(dimensionType);
            }

            var dimensionTypeRegistry = new NbtCompound("minecraft:dimension_type")
            {
                new NbtString("type", "minecraft:dimension_type"),
                dimensionTypesList
            };

            var root = new NbtCompound("")
            {
                dimensionTypeRegistry,
                // Optionally add other registries like biomes here
            };

            // Serialize the NBT data
            using (var ms = new MemoryStream())
            {
                root.WriteTag(ms);
                return ms.ToArray();
            }
        }

        private byte[] BuildDimension()
        {
            // Use the default or first world
            var world = WorldsOptions.Dimensions.First();

            var dimension = new NbtCompound("")
            {
                new NbtByte("piglin_safe", world.PiglinSafe ? (byte)1 : (byte)0),
                new NbtByte("natural", world.Natural ? (byte)1 : (byte)0),
                new NbtFloat("ambient_light", world.AmbientLight),
                new NbtString("infiniburn", world.Infiniburn),
                new NbtByte("respawn_anchor_works", world.RespawnAnchorWorks ? (byte)1 : (byte)0),
                new NbtByte("has_skylight", world.HasSkylight ? (byte)1 : (byte)0),
                new NbtByte("bed_works", world.BedWorks ? (byte)1 : (byte)0),
                new NbtString("effects", world.Effects),
                new NbtInt("logical_height", world.LogicalHeight),
                new NbtDouble("coordinate_scale", world.CoordinateScale),
                new NbtByte("ultrawarm", world.Ultrawarm ? (byte)1 : (byte)0),
                new NbtByte("has_ceiling", world.HasCeiling ? (byte)1 : (byte)0),
                // Add other properties as needed
            };

            // Serialize the NBT data
            using (var ms = new MemoryStream())
            {
                dimension.WriteTag(ms);
                return ms.ToArray();
            }
        }
    }
}