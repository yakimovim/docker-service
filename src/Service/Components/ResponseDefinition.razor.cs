using Microsoft.AspNetCore.Components;
using Service.Services;
using Service.Model;
using System.Linq;

namespace Service.Components
{
    public partial class ResponseDefinition
    {
        [Inject]
        public ResponseDefinitions Definitions { get; set; }

        public ResponseItem[] Items { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Items = Definitions.A.ToArray();
        }
    }
}
