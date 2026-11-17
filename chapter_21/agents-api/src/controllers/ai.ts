import type { RequestHandler } from "express";
import { z } from "zod";

const PromptBody = z.object({ prompt: z.string().min(1) });
export const echo: RequestHandler = async (req, res) => {
  const { prompt } = PromptBody.parse(req.body);
  res.json({ reply: prompt.toUpperCase() });
};
