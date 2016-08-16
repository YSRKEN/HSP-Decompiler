# HSP-Decompiler
Decompiler for Hot Soup Processor 2/3. Original source code written by Kitsutsuki.

## 概要
　[HSP](http://hsp.tv/)でコンパイルされたファイル(*.ax, *.exe, *.dpm)をソースファイル(*.hsp, *.as)に戻すソフトです。  
　HSPのバージョンは、HSP2およびHSP3に対応しています。  
　オリジナルのソフトは[きつつき](http://www.vector.co.jp/vpack/browse/person/an043697.html)さんによって書かれました(後述)。

## 歴史
### 作者
|ソフト名|作者|説明|
|--------|----|----|
|[HSP逆コンパイラ](http://www.vector.co.jp/soft/win95/prog/se390297.html)|[きつつき](http://www.vector.co.jp/vpack/browse/person/an043697.html)|オリジナルのソフト|
|[HSPdeco](https://osdn.jp/projects/hspdeco/)|[minorshift](https://osdn.jp/users/minorshift/)|オリジナルの改良Ver|
|[HSPdecom](http://stpr18.blogspot.jp/2015/10/hspdecohspelona.html)|[したぷる](https://www.blogger.com/profile/00794326060600750840)、[YSRKEN](https://github.com/YSRKEN)|HSPdecoの改良Ver|
|[HSPdecoのパッチ](http://vivibit.net/hspdeco/)|[xx2zz](http://vivibit.net/about/)|復号が失敗する際の対策パッチ|

### ソフト
|日付|ソフト名|バージョン|説明|
|----|--------|----------|----|
|2006/01/28|HSP逆コンパイラ|1.0|当初、シェアウェアとして公開された|
|2007/09/10|HSP逆コンパイラ|1.1|HSP3に対応された|
|2010/09/12|HSP逆コンパイラ|1.2|PDS・OSSになった他、バグ修正|
|2012/01/13|HSPdeco|1.0|axファイルのデコード機能を開放、バグ修正|
|2015/12/15|HSPdecom|1.0|変数名復元をサポート、辞書データ追加|
|2016/08/16|HSPdecom|1.1|パッチを全て付加、GitHubに上げ直し|

## 実行環境
　オリジナルのソフトは[.NET Framework 2.0](https://www.microsoft.com/ja-jp/download/details.aspx?id=1639)で動作します。  
　(ランタイム上で)高度な機能は全く使いませんので、後でもこれが踏襲されているようです。

## 開発環境
　オリジナルのソフトは恐らく[Visual Studio 2008](https://ja.wikipedia.org/wiki/Microsoft_Visual_Studio#Visual_Studio_2008)で書かれたのでしょう。  
　ただし、わざわざ古いVerのVSを入れるのが面倒なので、パッチ当ては[VS2015 Community](https://www.visualstudio.com/ja-jp/products/visual-studio-community-vs.aspx)で行いました。

## ライセンス
　PDSライセンス(HSPdecoはzlib/libpngライセンス。詳しくはLICENSEファイルを参照)
