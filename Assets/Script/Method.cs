class Method{
	public string NameCheck(string Name){
        if ((Name == null) || (Name.Length == 0)){
            return "文字を入力してください";
        }
        else if(Name.Length < 2 || 8 < Name.Length){
            return "2~8文字でお願いします";
        }else if(Name.Contains(" ")){
            return "空白文字が含まれています";
        }
        else{
            return "";
        }
	}

    
}