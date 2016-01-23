namespace CommonTypes.Interfaces {
    /// <summary>
    ///     action system is the common type of dicision making agent component
    /// </summary>
    public interface IActionSystem {
        /// <summary>
        ///     get next valid action
        /// </summary>
        /// <returns>IGoapAction</returns>
        IAction GetNextAction();

       
    }
}