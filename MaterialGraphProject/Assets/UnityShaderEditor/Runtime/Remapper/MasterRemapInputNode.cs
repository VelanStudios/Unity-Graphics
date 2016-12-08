using System.Collections.Generic;
using System.Linq;
using UnityEngine.Graphing;

namespace UnityEngine.MaterialGraph
{
    [Title("Remapper/Remap Input Node")]
    public class MasterRemapInputNode : AbstractSubGraphIONode
    {
        public MasterRemapInputNode()
        {
            name = "Inputs";
        }

        public override int AddSlot()
        {
            var nextSlotId = GetOutputSlots<ISlot>().Count() + 1;
            AddSlot(new MaterialSlot(-nextSlotId, "Input " + nextSlotId, "Input" + nextSlotId, SlotType.Output, SlotValueType.Vector4, Vector4.zero));

            if (onModified != null)
            {
                onModified(this, ModificationScope.Graph);
            }

            return -nextSlotId;
        }

        public override void RemoveSlot()
        {
            var lastSlotId = GetOutputSlots<ISlot>().Count();
            if (lastSlotId == 0)
                return;

            RemoveSlot(-lastSlotId);

            if (onModified != null)
            {
                onModified(this, ModificationScope.Graph);
            }

        }

        public override void GeneratePropertyUsages(ShaderGenerator visitor, GenerationMode generationMode)
        {
            if (generationMode == GenerationMode.ForReals)
                return;

            foreach (var slot in GetOutputSlots<MaterialSlot>())
            {
                var outDimension = ConvertConcreteSlotValueTypeToString(slot.concreteValueType);
                visitor.AddShaderChunk("float" + outDimension + " " + GetVariableNameForSlot(slot.id) + ";", true);
            }
        }

        public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
        {
            base.CollectPreviewMaterialProperties(properties);
            foreach (var slot in GetOutputSlots<MaterialSlot>())
            {
                properties.Add(
                    new PreviewProperty
                {
                    m_Name = GetVariableNameForSlot(slot.id),
                    m_PropType = PropertyType.Vector4,
                    m_Vector4 = slot.defaultValue
                }
                    );
            }
        }
    }
}
