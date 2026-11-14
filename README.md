# FontNao-ru

[![Build and Release](https://github.com/denpadokei/FontNao-ru/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/denpadokei/FontNao-ru/actions/workflows/build-and-release.yml)

Beat Saber のフォントを差し替える MOD です。`UserData` に置いたフォントを既定フォントとして使わせたり、フォールバックフォントとして追加したりできます。

## 機能

- `UserData` に置いたフォントで既定フォント (Teko-Medium SDF 系) を置き換え
- 追加のフォントをすべての TMP_FontAsset のフォールバックに登録し、未収録文字の豆腐を解消
- フォントは `.assets` (AssetBundle) から読み込み
- 楽曲リストのセルで、曲名がはみ出さないように自動サイズ調整

## 動作環境

| 項目 | 内容 |
| --- | --- |
| Beat Saber | 1.42.0 |
| 依存 MOD | BSIPA 4.3.7 / BeatSaberMarkupLanguage 1.14.1 / BS Utils 1.14.3 |

## インストール

1. Beat Saber を 1.42.0 に合わせる (BSManager などのバージョン管理ツールを推奨)
2. BSIPA と上記の依存 MOD をインストールする
3. [Releases](https://github.com/denpadokei/FontNao-ru/releases) から `FontNao_ru-0.2.1-bs1.42.0.zip` をダウンロードし、`Plugins` フォルダをゲームフォルダへ上書きコピーする

## 使い方

フォントは `.assets` 形式 (AssetBundle) で配置します。詳しくは [ESCFontProject](https://github.com/denpadokei/ESCFontProject) を参照してください。

- `UserData\FontNao_ru\FontAssets\Main` に置いたフォントが、主要な既定フォントの置き換えに使われます
- `UserData\FontNao_ru\FontAssets\FallBack` に置いたフォントが、すべてのフォントのフォールバックとして追加されます

フォルダは初回起動時に自動作成されます。フォントはゲーム起動時に読み込まれるため、追加した場合は Beat Saber を再起動してください。

変更前:

![スクリーンショット (27)](https://github.com/denpadokei/FontNao-ru/assets/55026301/7c8afc82-244c-460c-8760-b33724595db7)

変更後:

![スクリーンショット (28)](https://github.com/denpadokei/FontNao-ru/assets/55026301/bbb2b0d1-f575-4565-8e51-e46171fdeff3)

## ビルド

```powershell
git clone https://github.com/denpadokei/FontNao-ru.git
cd FontNao-ru
dotnet build .\FontNao-ru\FontNao-ru.csproj -c Release
```

`BeatSaberDir` に Beat Saber のインストール先を設定すると、ゲームの参照アセンブリを解決できます。

GitHub Actions の `build-and-release.yml` が push ごとにビルドし、`X.X.X` 形式のタグを push するとリリースを作成します。

## ライセンス

MIT
