using System;
using System.Collections.Generic;
using System.Linq;

namespace WowheadModelLoader
{
    // Это я сделал. Позволяет эмулировать отложенное выполнение делегатов в тех местах где идет загрузка данных с сервера через ajax,
    // потому что их создание модели глючит если загрузка происходит мгновенно
    // (конкретно после последнего WhModel.AddItem() не вызывается WhModel.UpdateMeshes())
    public static class WhDefferedList
    {
        public static void Execute(string block = null)
        {
            while (_list.Any(x => x.block == block))
            {
                var actionsToRun = _list.Where(x => x.block == block).Select(x => x.action).ToArray();
                _list.RemoveAll(x => x.block == block);

                foreach (var action in actionsToRun)
                    action?.Invoke();
            }
        }

        public static void Add(Action methodToRunLater, string block = null)
        {
            _list.Add(new ActionWithBlock() { action = methodToRunLater, block = block });
        }

        private static List<ActionWithBlock> _list = new List<ActionWithBlock>();

        private struct ActionWithBlock
        {
            public string block;
            public Action action;
        }
    }
}
