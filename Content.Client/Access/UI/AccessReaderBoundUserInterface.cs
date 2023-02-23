using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client.Access.UI
{
    public sealed class AccessReaderBoundUserInterface : BoundUserInterface
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;

        public AccessReaderBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        private AccessReaderWindow? _window;

        protected override void Open()
        {
            base.Open();
            List<string> accessLevels;

            if (_entityManager.TryGetComponent<AccessReaderComponent>(Owner.Owner, out var accessreader))
            {
                accessLevels = accessreader.AccessLevels;
                accessLevels.Sort();
            }
            else
            {
                accessLevels = new List<string>();
            }

            _window = new AccessReaderWindow(this, _prototypeManager, accessLevels) {Title = _entityManager.GetComponent<MetaDataComponent>(Owner.Owner).EntityName};

            _window.OnClose += Close;
            _window.OpenCentered();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            _window?.Dispose();
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            var castState = (AccessReaderBoundUserInterfaceState) state;
            _window?.UpdateState(castState);
        }

        public void SubmitData(List<string> newAccessList, bool inverted)
        {
            SendMessage(new WriteToTargetAccessReaderMessage(newAccessList, inverted));
        }
    }
}
