using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Service.Components.ResponseSpecifications
{
    public abstract class ResponseSpecificationComponentBase : ComponentBase
    {
        [Parameter]
        public int Index { get; set; }

        [Parameter]
        public int Total { get; set; }

        [Parameter]
        public Action<int> Delete { get; set; }

        [Parameter]
        public Action<int> MoveUp { get; set; }

        [Parameter]
        public Action<int> MoveDown { get; set; }

        protected bool CanDeleteItem => Total > 1;

        protected internal Dictionary<string, object> DeleteButtonState => new Dictionary<string, object>
        {
            { "disabled", !CanDeleteItem }
        };

        protected internal void DeleteItem()
        {
            Delete(Index);
        }

        protected bool CanMoveItemUp => Index > 0;

        protected internal Dictionary<string, object> MoveUpButtonState => new Dictionary<string, object>
        {
            { "disabled", !CanMoveItemUp }
        };

        protected internal void MoveItemUp()
        {
            MoveUp(Index);
        }

        protected bool CanMoveItemDown => Index < (Total - 1);

        protected internal Dictionary<string, object> MoveDownButtonState => new Dictionary<string, object>
        {
            { "disabled", !CanMoveItemDown }
        };

        protected internal void MoveItemDown()
        {
            MoveDown(Index);
        }
    }
}
