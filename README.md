# TinyGPT
A lightweight, educational implementation of a Transformer-based language model built from scratch in C#. Inspired by  ["Attention Is All You Need"](https://arxiv.org/pdf/1706.03762) and ["Let's build a Storyteller"]( https://github.com/karpathy/LLM101n) and [Build a (Tiny) LLM from Scratch](https://algo.monster/courses/llm/llm_course_introduction). It demonstrates 
- core LLM concepts
- Transformer logic.

No external libraries.

## Features of the transformer 

✅ Tokenization & vocabulary (HashMap / Dictionary)

✅ Embeddings + positional encoding

✅ Multi-head attention (graph-based reasoning)

✅ Feed-forward network

✅ Text generation:

   - Sampling (prefix sum + binary search)

   - Beam search (greedy + top-K)

✅ Sliding window sequences

✅ Clean separation:

  - Domain (entities, value objects)

  - Application (training, inference)

  - Infrastructure (math ops)

## High-level flow
```
Text
 ↓
Tokenization (Vocabulary)
 ↓
Embeddings + Positional Encoding
 ↓
Transformer Block
   ├── Multi-Head Attention
   ├── Residual + LayerNorm
   ├── Feed Forward
 ↓
Linear Layer + Softmax
 ↓
Next Token Prediction
```

