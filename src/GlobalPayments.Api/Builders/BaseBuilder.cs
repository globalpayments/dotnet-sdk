namespace GlobalPayments.Api.Builders {
    public abstract class BaseBuilder<TResult> {
        internal Validations Validations { get; set; }

        public BaseBuilder() {
            Validations = new Validations();
            SetupValidations();
        }

        public virtual TResult Execute(string configName = "default") {
            Validations.Validate(this);
            return default(TResult);
        }

        protected virtual void SetupValidations() { }
    }
}
