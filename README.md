# Hierarchy

Unity の Hierarchy ウィンドウを拡張するパッケージです。

---

### インストール方法

Windows の場合はコマンドプロンプト、Mac、Linux の場合はターミナルで  
`git` のコマンドが実行可能な状態か確認してから行って下さい。

パッケージをインストールしたい Unity プロジェクトの  
Packages/manifest.json をテキストエディタで開きます。

開いたら "dependencies" ノード内に以下を追加してください。

```
"com.devknit.hierarchy": "https://github.com/devknit/HierarchyPackage.git",
```

![](Documentation/install.gif)

追加した状態を保存後、Unity にフォーカスを当てると
パッケージのインストールが行われます。

インストールが正しく行わると `Projectウィンドウ` の `Packages` に  
`Finder` が追加されていることが確認できます。

![](Documentation/install.png)

---

### 設定ウィンドウの起動方法

Unity のメニューに Tools > Hierarchy > Settings から起動できます。

---

### アップデート方法

パッケージがインストールされている Unity プロジェクトの
Packages/manifest.json をテキストエディタで開きます。

開いたら "lock" ノード内にある "com.devknit.hierarchy" を削除してください。

![](Documentation/update.gif)

削除した状態を保存後、Unity にフォーカスを当てると
パッケージのアップデートが行われます。
