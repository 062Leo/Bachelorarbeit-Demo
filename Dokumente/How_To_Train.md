# How To Train

## 1. Virtuelle Umgebung und TensorBoard

1. **Venv im Projekt-Root starten**

   ```bash
   venv\Scripts\activate
   ```

2. **TensorBoard für Ergebnisse starten**

   ```bash
   tensorboard --logdir results
   ```

---

## 2. Automatisches Training starten

```bash
python Training_Automation.py
```

Das Skript `Training_Automation.py` steuert die Trainingsläufe und passt Konfigurationen automatisch an.

---

## 3. Manuelles Training starten (im venv)

Grundbefehl:

```bash
mlagents-learn config\**.yaml --run-id=(Name)
```

### Wichtige Optionen für manuelles Training

1. **Von vorherigem Training starten/ableiten**

   ```bash
   --initialize-from=(name_in_Results_Folder)
   ```

2. **Nur ausführen, kein Training (Inference-Modus)**

   ```bash
   --inference
   ```

3. **Vorhandene Run-ID überschreiben**

   ```bash
   --force
   ```

4. **Vorheriges Training fortsetzen**

   ```bash
   --resume
   ```

5. **Training mit Build statt im Editor**

   ```bash
   --env="Builds\**.exe"
   ```

---

## 4. Beispielbefehle

### Neues Training mit Build

```bash
mlagents-learn config\**.yaml --run-id=(name) --env="Builds\**.exe"
```

### Fertiges Modell ohne weiteres Training ausführen

Zum Testen der Generalisierungsfähigkeit im Generalisierungslevel:

```bash
mlagents-learn config\Generalisierung\Generalisierung.yaml \
  --run-id=(Name) \
  --env="Builds\**.exe" \
  --inference \
  --initialize-from=(modelNameDasManTestenWill)
```
