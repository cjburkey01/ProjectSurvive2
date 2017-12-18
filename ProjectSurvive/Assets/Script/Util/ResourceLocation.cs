public class ResourceLocation {

	public readonly string domain;
	public readonly string path;

	public ResourceLocation(string domain, string path) {
		this.domain = domain;
		path = path.Replace('\\', '/');
		while (path.StartsWith("/", System.StringComparison.Ordinal)) {
			path = path.Substring(1);
		}
		while (path.EndsWith("/", System.StringComparison.Ordinal)) {
			path = path.Substring(0, path.Length - 1);
		}
		this.path = path;
	}

}