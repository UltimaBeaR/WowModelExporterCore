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

        // Index родителя
        public short Parent { get; set; }

        // Думаю что это ид/индекс сабмеша к которому относится кость. типа чтобы проверить какие кости можно не создавать, если сабмеш не показывается
        public ushort Mesh { get; set; }

        // Уникальный идентификатор. Один и тот же для одинаковых костей в разных скелетах
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

        public bool SkipUpdate { get; set; }

        // LastUpdated* переменные сделал я. Это аналог оригинальной матрицы кости, но разбитой по компонентам.
        // они заполняются после вызова Update(time) то есть во время проигрывания текущей анимации

        public Vec3 LastUpdatedTranslation { get; set; }
        public Vec4 LastUpdatedRotation { get; set; }
        public Vec3 LastUpdatedScale { get; set; }

        public void Hide()
        {
            Hidden = true;

            //for (var i = 0; i < 16; ++i)
            //    self.matrix[i] = 0
        }

        public void Update(int time)
        {
            if (Hidden)
            {
                Hide();
                return;
            }

            if (Updated || SkipUpdate)
                return;

            Updated = true;

            if (Model == null || Model.Animations == null || Model.AnimPaused)
                return;

            //mat4.identity(self.matrix);

            var anim = Model.CurrentAnimation;

            if (anim == null)
                return;

            var billboard = (Flags & 8) != 0;
            var transUsed = WhAnimatedVec3.IsUsed(Translation, anim.Index);
            var rotUsed = WhAnimatedQuat.IsUsed(Rotation, anim.Index);
            var scaleUsed = WhAnimatedVec3.IsUsed(Scale, anim.Index);

            if (transUsed || rotUsed || scaleUsed || billboard)
            {
                //mat4.translate(self.matrix, self.matrix, self.pivot);

                if (transUsed)
                {
                    // Запоминаю текущий translation вместо модификации матрицы (как в оригинале)

                    LastUpdatedTranslation = WhAnimatedVec3.GetValue(Translation, anim.Index, time);

                    //Wow.AnimatedVec3.getValue(self.translation, anim.index, time, self.tmpVec);
                    //mat4.translate(self.matrix, self.matrix, self.tmpVec)
                }

                if (rotUsed)
                {
                    // Запоминаю текущий rotation вместо модификации матрицы (как в оригинале)

                    // mat4.fromQuat + mat4.transpose (создать матрицу из квартерниона и инвертировать ее) это то же самое что инверт квартерниона, а затем создание матрицы из полученного квартерниона
                    // но так как я матрицы не использую, я просто инвертирую квартернион и сохраняю его
                    LastUpdatedRotation = Quat.Invert(WhAnimatedQuat.GetValue(Rotation, anim.Index, time));

                    //Wow.AnimatedQuat.getValue(self.rotation, anim.index, time, self.tmpQuat);
                    //mat4.fromQuat(self.tmpMat, self.tmpQuat);
                    //mat4.transpose(self.tmpMat, self.tmpMat);
                    //mat4.multiply(self.matrix, self.matrix, self.tmpMat)
                }

                if (scaleUsed)
                {
                    // Запоминаю текущий scale вместо модификации матрицы (как в оригинале)

                    LastUpdatedScale = WhAnimatedVec3.GetValue(Scale, anim.Index, time);

                    //Wow.AnimatedVec3.getValue(self.scale, anim.index, time, self.tmpVec);
                    //mat4.scale(self.matrix, self.matrix, self.tmpVec)
                }

                if (billboard)
                {
                    //var yRot = -self.model.renderer.zenith + Math.PI / 2;
                    //var zRot;
                    //if (self.model.model.type == Wow.Types.ITEM)
                    //{
                    //    zRot = self.model.renderer.azimuth - Math.PI
                    //    }
                    //else
                    //{
                    //    zRot = self.model.renderer.azimuth - Math.PI * 1.5
                    //  }
                    //mat4.identity(self.matrix);
                    //mat4.translate(self.matrix, self.matrix, self.pivot);
                    //mat4.rotateZ(self.matrix, self.matrix, zRot);
                    //mat4.rotateY(self.matrix, self.matrix, yRot)
                }

                //mat4.translate(self.matrix, self.matrix, vec3.negate(self.tmpVec, self.pivot))
            }

            if (Parent > -1)
            {
                Model.Bones[Parent].Update(time);

                // По сути это значит что translation/rotation/scale которые в этой кости вычеслены будут являться локальными и чтобы получить глобальные,
                // нужно учесть еще из parent кости тоже
                //mat4.multiply(self.matrix, self.model.bones[self.parent].matrix, self.matrix)
            }

            //vec3.transformMat4(self.transformedPivot, self.pivot, self.matrix)
        }
    }
}
