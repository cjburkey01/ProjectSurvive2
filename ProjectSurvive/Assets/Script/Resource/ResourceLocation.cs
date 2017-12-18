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

	public override string ToString() {
		return domain + ":" + path;
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(this, obj)) {
			return true;
		}
		if (!(obj is ResourceLocation)) {
			return false;
		}
		ResourceLocation other = (ResourceLocation) obj;
		return other.domain.Equals(this.domain) && other.path.Equals(this.path);
	}

	public override int GetHashCode() {
		unchecked {
			int hash = 17;
			hash = hash * 23 + domain.GetHashCode();
			hash = hash * 23 + path.GetHashCode();
			return hash;
		}
	}

}