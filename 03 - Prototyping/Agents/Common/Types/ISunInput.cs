namespace Common.Types {
  
  /// <summary>
  ///   Storage object for sunshine sensor input.
  /// </summary>
  public interface ISunInput {

    /// <summary>
    ///   Return the sunshine value.
    /// </summary>
    /// <returns>A boolean: True, if sun shines, else false.</returns>
    bool GetSunshine();
  }



}