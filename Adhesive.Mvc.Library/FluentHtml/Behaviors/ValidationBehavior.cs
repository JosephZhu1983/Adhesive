using System;
using System.Web.Mvc;
using Adhesive.Mvc.Library.FluentHtml.Elements;
using Adhesive.Mvc.Library.FluentHtml.Html;

namespace Adhesive.Mvc.Library.FluentHtml.Behaviors
{
    public class ValidationBehavior : ThreadSafeBehavior
    {
        private const string defaultValidationCssClass = "input-validation-error";
        private readonly Func<ModelStateDictionary> modelStateDictionaryFunc;
        private readonly string validationErrorCssClass;

        public ValidationBehavior(Func<ModelStateDictionary> modelStateDictionaryFunc)
            : this(modelStateDictionaryFunc, defaultValidationCssClass) { }

        public ValidationBehavior(Func<ModelStateDictionary> modelStateDictionaryFunc, string validationErrorCssClass)
        {
            this.modelStateDictionaryFunc = modelStateDictionaryFunc;
            this.validationErrorCssClass = validationErrorCssClass;
        }

        protected override void DoExecute(IElement element)
        {
            var name = element.GetAttr(HtmlAttribute.Name);
            var supportsModelState = element as ISupportsModelState;

            if (name == null || supportsModelState == null)
            {
                return;
            }

            ModelState state;
            if (modelStateDictionaryFunc().TryGetValue(name, out state))
            {
                if (HasErrors(state))
                {
                    element.Builder.AddCssClass(validationErrorCssClass);
                }

                if (state.Value != null)
                {
                    supportsModelState.ApplyModelState(state);
                }
            }
        }

        private bool HasErrors(ModelState state)
        {
            return state.Errors != null && state.Errors.Count > 0;
        }
    }
}