import torch
from datasets import load_dataset
from transformers import T5Tokenizer, T5ForConditionalGeneration, Trainer, TrainingArguments

# Check if GPU is available
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

# Load SAMSum dataset
dataset = load_dataset("sentence-transformers/wikihow", trust_remote_code=True)

# Load T5 tokenizer and model
tokenizer = T5Tokenizer.from_pretrained("t5-small")  # Change to "t5-base" for better performance
model = T5ForConditionalGeneration.from_pretrained("t5-small").to(device)

# Preprocess the data
def preprocess_function(batch):
    # Add the summarization prefix
    inputs = ["summarize: " + text for text in batch["text"]]
    model_inputs = tokenizer(inputs, max_length=512, truncation=True, padding="max_length")

    # Tokenize summaries as targets
    with tokenizer.as_target_tokenizer():
        labels = tokenizer(batch["summary"], max_length=128, truncation=True, padding="max_length")
    
    model_inputs["labels"] = labels["input_ids"]
    return model_inputs

# Apply preprocessing to the dataset
tokenized_dataset = dataset.map(preprocess_function, batched=True)

# Split dataset
train_dataset = tokenized_dataset["train"].select(range(15000))

# Define training arguments
training_args = TrainingArguments(
    output_dir="./results",
    learning_rate=2e-5,
    per_device_train_batch_size=4,
    per_device_eval_batch_size=4,
    num_train_epochs=3,
    weight_decay=0.01,
    save_total_limit=1,
    logging_dir="./logs",
    logging_steps=10,
    fp16=torch.cuda.is_available(),
)

# Define Trainer
trainer = Trainer(
    model=model,
    args=training_args,
    train_dataset=train_dataset,
    tokenizer=tokenizer,
)

# Fine-tune the model
trainer.train()

# Save the fine-tuned model
model.save_pretrained("./t5_summarization_long")
tokenizer.save_pretrained("./t5_summarization_long")
