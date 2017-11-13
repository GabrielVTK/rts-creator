using System.Collections.Generic;

public class ScriptAttributes {

	public Dictionary<string, float> attributesList;

    public ScriptAttributes() {
        this.attributesList = new Dictionary<string, float>();
    }

    public void AddAttribute(string name, float value) {
        this.attributesList.Add(name, value);
    }

    public float GetAttribute(string name) {
        return this.attributesList[name];
    }

}
