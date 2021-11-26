using Microsoft.AspNetCore.Components;
using Service.Services;
using Service.Model;
using System.Linq;
using System.Collections.Generic;

namespace Service.Components
{
    public partial class ResponseDefinition
    {
        [Inject]
        public ResponseDefinitions Definitions { get; set; }

        public List<ResponseItem> Items { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Items = Definitions.A
                .Select(i => i.Clone())
                .ToList();
        }

        private void DeleteItem(int index, ResponseItem item)
        {
            Items.RemoveAt(index);
            StateHasChanged();
        }

        private void MoveItemUp(int index)
        {
            var item = Items[index];
            Items.RemoveAt(index);
            Items.Insert(index - 1, item);
            StateHasChanged();
        }

        private void MoveItemDown(int index)
        {
            var item = Items[index];
            Items.RemoveAt(index);
            Items.Insert(index + 1, item);
            StateHasChanged();
        }

        private void AddResponseItem()
        {
            Items.Add(new Response());
            StateHasChanged();
        }

        private void ApplyChanges()
        {
            Definitions.A = Items
                .Select(i => i.Clone())
                .ToArray();
        }
    }
}
