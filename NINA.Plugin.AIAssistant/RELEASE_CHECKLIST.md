# NINA Plugin Release Checklist - AI Assistant v1.0.0.3

## ✅ What's Correct

### Package Structure
- ✅ ZIP archive created (`AIAssistant-1.0.0.3.zip` - 2.1 MB)
- ✅ All dependencies included (Azure.AI.Inference, OpenAI, etc.)
- ✅ manifest.json included in package
- ✅ Runtime dependencies included (runtimes/browser folder)

### AssemblyInfo Metadata
- ✅ AssemblyTitle: "AI Assistant"
- ✅ Guid: "af5e2826-e3b4-4b9c-9a1a-1e8d7c8b6a9e"
- ✅ AssemblyVersion: 1.0.0.3
- ✅ MinimumApplicationVersion: 3.0.0.0
- ✅ Repository: https://github.com/michelebergo/nina-ai-assistant
- ✅ License: MIT
- ✅ FeaturedImageURL: GitHub raw icon.png
- ✅ LongDescription: Multi-provider AI support

## ⚠️ Issues Found

### 1. **manifest.json Version Mismatch** 🔴 CRITICAL
**Current:** Version 1.0.0.1 in manifest.json
**Expected:** Version 1.0.0.3 (matches AssemblyInfo.cs)
**Impact:** Plugin manager will show wrong version

### 2. **manifest.json Name Case** 🟡 MINOR
**Current:** "ai assistant" (lowercase)
**Expected:** "AI Assistant" (matches AssemblyTitle)
**Impact:** Display inconsistency

### 3. **Missing Plugin Manifest Submission** 📝
**Status:** Not yet submitted to NINA plugin manifest repository
**Required:** Create manifest file for https://github.com/isbeorn/nina.plugin.manifests
**Path:** `manifests/a/AI Assistant/3.0/1.0.0.3/manifest.json`

### 4. **Installer Type Not Specified** 🔴 CRITICAL
**Current:** Not specified in manifest
**Required:** `"Installer": { "Type": "ARCHIVE", "URL": "...", "Checksum": "...", "ChecksumType": "SHA256" }`
**Impact:** Plugin manager won't know how to install

### 5. **GitHub Release Not Created** 📝
**Status:** No release tag `v1.0.0.3` on GitHub
**Required:** Create GitHub release with:
  - Tag: `v1.0.0.3`
  - Upload: `AIAssistant-1.0.0.3.zip`
  - Release notes from RELEASE_NOTES.md

## 🔧 Required Actions Before Release

### Immediate Fixes (In Code)
1. **Update manifest.json version:**
   - Change Version.Build from 1 to 3
   - Change Name from "ai assistant" to "AI Assistant"

2. **Build final package:**
   - Clean build
   - Create new ZIP with correct manifest

### GitHub Setup
3. **Create GitHub Release:**
   ```bash
   git tag v1.0.0.3
   git push origin v1.0.0.3
   ```
   - Upload `AIAssistant-1.0.0.3.zip`
   - Add RELEASE_NOTES.md as description

4. **Calculate package checksum:**
   ```powershell
   Get-FileHash AIAssistant-1.0.0.3.zip -Algorithm SHA256
   ```

### NINA Manifest Repository
5. **Create plugin manifest for submission:**
   - Use the CreateManifest.ps1 tool OR
   - Create manual JSON following schema
   - Include:
     * All required fields (Name, Identifier, Version, etc.)
     * Installer.Type: "ARCHIVE"
     * Installer.URL: GitHub release download URL
     * Installer.Checksum: SHA256 hash
     * Installer.ChecksumType: "SHA256"

6. **Submit to manifest repository:**
   - Fork https://github.com/isbeorn/nina.plugin.manifests
   - Add manifest to `manifests/a/AI Assistant/3.0/1.0.0.3/manifest.json`
   - Create Pull Request

## 📋 NINA Platform Requirements Summary

### Required Files in ZIP
- ✅ Plugin DLL (NINA.Plugin.AIAssistant.dll)
- ✅ All dependency DLLs
- ✅ manifest.json
- ✅ Config files (.dll.config, .deps.json)

### manifest.json Required Fields
- ✅ Name (must match AssemblyTitle)
- ✅ Identifier (must match Guid)
- ✅ Author
- ✅ Version (Major.Minor.Patch.Build) - **NEEDS FIX**
- ✅ MinimumApplicationVersion
- ✅ ShortDescription
- ❌ Installer.Type (MISSING)
- ❌ Installer.URL (MISSING)
- ❌ Installer.Checksum (MISSING)
- ❌ Installer.ChecksumType (MISSING)
- ✅ Repository
- ✅ License
- ✅ LicenseURL

### Distribution Methods
1. **Official Plugin Repository** (Recommended):
   - Submit manifest to https://github.com/isbeorn/nina.plugin.manifests
   - Users can install via NINA Plugin Manager
   - Automatic updates

2. **Manual Distribution**:
   - Users extract ZIP to `%localappdata%\NINA\Plugins\3.0.0\AI Assistant\`
   - No automatic updates

## 🎯 Next Steps

**To complete release:**
1. Fix manifest.json (version + name)
2. Rebuild package
3. Create GitHub release
4. Calculate SHA256 checksum
5. Create manifest file with installer info
6. Submit to NINA manifest repository

**OR for quick manual release:**
1. Fix manifest.json
2. Rebuild package
3. Upload to GitHub releases
4. Document manual installation instructions
