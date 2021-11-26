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

        private void DeleteItem(int index)
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

        private void AddProbableResponseItem()
        {
            Items.Add(new ProbableResponse { Probability = 0.5 });
            StateHasChanged();
        }

        private void AddDelayItem()
        {
            Items.Add(new Delay { DelayDurationInSeconds = 3 });
            StateHasChanged();
        }

        private void AddProbableDelayItem()
        {
            Items.Add(new ProbableDelay { Probability = 0.5, DelayDurationInSeconds = 3 });
            StateHasChanged();
        }

        private void AddCallItem()
        {
            Items.Add(new Call());
            StateHasChanged();
        }

        private void AddProbableCallItem()
        {
            Items.Add(new ProbableCall { Probability = 0.5 });
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
