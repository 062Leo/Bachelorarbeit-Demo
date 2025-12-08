# Installation Guide

## Voraussetzungen
- Python 3.10 (erforderlich, da neuere Versionen `distutils` nicht mehr unterstützen, aber benötigt wird)

## Installationsschritte

### 1. Virtuelle Umgebung erstellen
```bash
py -3.10 -m venv venv
```

### 2. Virtuelle Umgebung aktivieren
```bash
venv\Scripts\activate
```

### 3. Abhängigkeiten installieren
```bash
pip install -r requirements.txt
```
Dies installiert alle erforderlichen Pakete, einschließlich ML Agents und weitere Abhängigkeiten.

### 4. Installation testen
```bash
mlagents-learn --help
```
Dieser Befehl sollte die Hilfe für ML Agents anzeigen und bestätigt, dass die Installation erfolgreich war.

## Hinweise
- Stelle sicher, dass Python 3.10 auf deinem System installiert ist
- Die virtuelle Umgebung muss vor der Installation der Abhängigkeiten aktiviert sein
- Bei Problemen überprüfe, dass alle Anforderungen in `requirements.txt` kompatibel sind
