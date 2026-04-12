export class ArgumentNullException {
  public static ThrowIfNullOrUndefined(value: any, paramName: string): void {
    if (value === null || value === undefined) {
        throw new Error(`Argument '${paramName}' cannot be null or undefined.`);
    }
  }
}