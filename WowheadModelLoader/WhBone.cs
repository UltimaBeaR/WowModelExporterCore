using System.IO;

namespace WowheadModelLoader
{
    public class WhBone
    {
        public WhBone(WhModel model, int index, BinaryReader r)
        {
            Model = model;
            Index = index;
            KeyId = r.ReadInt32();
            Flags = r.ReadUInt32();
            Parent = r.ReadInt16();
            Mesh = r.ReadUInt16();
            Id = r.ReadUInt32();
            Pivot = new Vec3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            Translation = WhAnimatedVec3.ReadSet(r);
            Rotation = WhAnimatedQuat.ReadSet(r);
            Scale = WhAnimatedVec3.ReadSet(r);
            Hidden = false;
            Updated = false;

            // Не стал делать
            //self.transformedPivot = vec3.create();
            //self.matrix = mat4.create();
            //self.tmpVec = vec3.create();
            //self.tmpQuat = quat.create();
            //self.tmpMat = mat4.create();
        }

        public WhModel Model { get; set; }

        // ToDo: Странно что int, это индекс в массиве костей у модели. в Attachments у модели есть индекс кости по которому она адресуется в этом массиве и там оно int (читается так ридером)
        // в то же время тут есть Parent который short и это тоже индекс кости в этом же массиве (он читается ридером как short)
        // оставлю так, но скорее всего он всегда short, да и не может быть костей больше размерности short
        public int Index { get; set; }

        public int KeyId { get; set; }
        public uint Flags { get; set; }
        public short Parent { get; set; }
        public ushort Mesh { get; set; }
        public uint Id { get; set; }

        // Похоже на исходную позицию (Bind pose) кости, то есть позиция кости в которой модель была привязана к скелету (в этой позиции кости не будет морфинга вершин скелетом)
        public Vec3 Pivot { get; set; }

        // По всей видимости это анимации для данной модели (Translation/Rotation/Scale массивы). Каждый итем в массиве - отдельная анимация.
        // А в списке анимаций модели который читается отдельно наверно какие-то метаданные, а тут основное мясо.
        // Внутри каждого анимированного Vec3/Quat есть массив времени/значений видимо для кадров анимации.

        public WhAnimatedVec3[] Translation { get; set; }
        public WhAnimatedQuat[] Rotation { get; set; }
        public WhAnimatedVec3[] Scale { get; set; }

        public bool Hidden { get; set; }
        public bool Updated { get; set; }

        // ToDo: есть еще методы update() и hide() не уверен что оно мне надо, пока не буду делать, вроде update() нужен для изменения состояния анимации
    }
}
